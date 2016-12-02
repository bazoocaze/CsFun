/* File.....: FdSelect.h
 * Author...: Jose Ferreira
 * Date.....: 2016-11-24 14:15
 * Purpose..: File descriptor status selector
 * */


#include <sys/select.h>
#include <string.h>
#include <errno.h>

#include "FdSelect.h"
#include "Util.h"
#include "IO.h"
#include "Logger.h"


#define SELECT_POLL_TIME 1000
#define RET_SEL_TIMEOUT  0


/* Default constructor for an empty file descriptor selector. */
CFdSelect::CFdSelect()
{
	Clear();
}

int CFdSelect::FindFreeSlot(int fd)
{
	// Find same fd
	for(int n = 0; n < MAX_FD_LIST; n++)
		if(fd_list[n].fd == fd)
			return n;

	// Find another free slot
	for(int n = 0; n < MAX_FD_LIST; n++)
		if(fd_list[n].flags == 0)
			return n;

	// Can't find a free slot
	return RET_ERR;
}

int CFdSelect::FindFd(int fd)
{
	// Find the fd
	for(int n = 0; n < MAX_FD_LIST; n++)
		if(fd_list[n].fd == fd)
			return n;
	// Can't find the fd
	return RET_ERR;
}

/* Extract the updated state of a single fd from the fd sets. */
int CFdSelect::GetStatus(int fd, int iflags, fd_set* prfds, fd_set* pwfds, fd_set* pefds)
{
	if(iflags <= 0) return 0;

	int oflags = 0;

	if(HAS_FLAG(iflags, SEL_READ)  && FD_ISSET(fd, prfds))
		oflags |= SEL_READ;

	if(HAS_FLAG(iflags, SEL_WRITE) && FD_ISSET(fd, pwfds))
		oflags |= SEL_WRITE;

	if(HAS_FLAG(iflags, SEL_ERROR) && FD_ISSET(fd, pefds))
		oflags |= SEL_ERROR;

	return oflags;
}

/* Returns the string message for the last error code. */
const char * CFdSelect::GetLastErrMsg() const
{
	if(LastErr == RET_OK)
		return "";
	else
		return strerror(LastErr);
}

/* Clear the list of prepared file descriptors. */
void CFdSelect::Clear()
{
	LastErr = RET_OK;
	for(int n = 0; n < MAX_FD_LIST; n++)
	{
		fd_list[n].fd    = CLOSED_FD;
		fd_list[n].flags = 0;
	}
}

/* Prepared and add a file descriptor to the list
 * of prepared fds.
 * Flags can be SEL_READ, SEL_WRITE and SEL_ERROR.
 * Returns true on success, or false on out of free
 * fd slots (see the MAX_FD_LIST constant).
 * Update fd status using Wait(), and read updated
 * status using GetStatus(). */
bool CFdSelect::Add(int fd, int flags)
{
	int slot = FindFreeSlot(fd);

	if(slot == RET_ERR)
		return false;

	fd_list[slot].fd = fd;
	fd_list[slot].flags = flags;

	return true;
}

/* Removes a file descriptor from the list
 * of prepared fds. */
bool CFdSelect::Remove(int fd)
{
	int slot = FindFd(fd);

	if(slot != RET_ERR)
		fd_list[slot].flags = 0;

	return (slot != RET_ERR);
}

/* Read the result status of the prepared fd.
 * Returns the status, or 0 if the fd was
 * not selected/updated on Wait(). */
int CFdSelect::GetStatus(int fd)
{
	int slot = FindFd(fd);

	if(slot == RET_ERR)
		return 0;

	int iflags = fd_list[slot].flags;

	return GetStatus(fd, iflags, &rfds, &wfds, &efds);
}

/* Select/Wait for the list of prepared fds.
 * Use the Add() and Remove() to add and remove
 * fds to the list of prepared fds.
 * Returns the number of selected fds,
 * 0 on no fd/timeout or RET_ERR on error. */
int CFdSelect::WaitAll(int timeoutMillis)
{
	int maxFd   = -1;
	fd_set  fds[3];
	fd_set* pfds[3];
	int nflags[] = { SEL_READ, SEL_WRITE, SEL_ERROR };
	uint64_t timeout = millis() + timeoutMillis;

	do
	{
		// Zero the fd sets
		for(int type = 0; type < 3; type++)
		{
			FD_ZERO(&fds[type]);
			pfds[type] = NULL;
		}

		// Populate the fd sets
		for(int n = 0; n < MAX_FD_LIST; n++)
		{
			if(fd_list[n].fd < 0 || fd_list[n].flags <= 0) continue;

			int fd = fd_list[n].fd;
			int flags = fd_list[n].flags;
			maxFd = MAX(fd, maxFd);

			// Populate the sets according to the flags
			for(int type = 0; type < 3; type++)
			{
				if(HAS_FLAG(flags, nflags[type]))
				{
					pfds[type] = &fds[type];
					FD_SET(fd, pfds[type]);
				}
			}
		}

		int selectTime = (timeout - millis());
		selectTime = CLAMP(selectTime, 0, SELECT_POLL_TIME);

		// prepare the timeout
		struct timeval tv;
		MILLIS_TO_TIMEVAL(selectTime, tv);

		// select the file descriptor
		int retval = select(maxFd + 1, pfds[0], pfds[1], pfds[2], &tv);

		// if not timeout
		if(retval != RET_SEL_TIMEOUT)
			return retval;

	} while(millis() < timeout);

	return RET_SEL_TIMEOUT;
}


/* Wait/select state for a single file descriptor.
 * Returns the the state of the descriptor on success.
 * Returns 0 on timeout or RET_ERR on error. */
int CFdSelect::Wait(int fd, int flags, int timeoutMillis)
{
	int retval;
	fd_set  fds[3];
	fd_set* pfds[3];
	int nflags[] = { SEL_READ, SEL_WRITE, SEL_ERROR };	
	uint64_t timeout = millis() + timeoutMillis;

	do
	{
		for(int n = 0; n < 3; n++)
		{
			FD_ZERO(&fds[n]);
			pfds[n] = NULL;
			if(HAS_FLAG(flags, nflags[n]))
			{
				pfds[n] = &fds[n];
				FD_SET(fd, pfds[n]);
			}
		}

		int selectTime = (timeout - millis());
		selectTime = CLAMP(selectTime, 0, SELECT_POLL_TIME);

		// prepare the timeout
		struct timeval tv;
		MILLIS_TO_TIMEVAL(selectTime, tv);

		// select the file descriptor
		retval = select(fd + 1, pfds[0], pfds[1], pfds[2], &tv);

		// if not timeout
		if(retval != RET_SEL_TIMEOUT)
		{
			// return fd status on success
			if(retval != RET_ERR)
				return GetStatus(fd, flags, pfds[0], pfds[1], pfds[2]);

			return retval;
		}

	} while(millis() < timeout);

	return RET_SEL_TIMEOUT;
}
