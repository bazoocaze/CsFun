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



/* Default constructor for an empty file descriptor selector. */
FdSelect::FdSelect()
{
	Clear();
}

int FdSelect::FindFreeSlot(int fd)
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

int FdSelect::FindFd(int fd)
{
	// Find the fd
	for(int n = 0; n < MAX_FD_LIST; n++)
		if(fd_list[n].fd == fd)
			return n;
	// Can't find the fd
	return RET_ERR;
}

/* Extract the updated state of a single fd from the fd sets. */
int FdSelect::GetStatus(int fd, int iflags, fd_set* prfds, fd_set* pwfds, fd_set* pefds)
{
	int oflags = 0;
	if(HAS_FLAG(iflags, SEL_READ)  && FD_ISSET(fd, prfds)) oflags |= SEL_READ;
	if(HAS_FLAG(iflags, SEL_WRITE) && FD_ISSET(fd, pwfds)) oflags |= SEL_WRITE;
	if(HAS_FLAG(iflags, SEL_ERROR) && FD_ISSET(fd, pefds)) oflags |= SEL_ERROR;
	return oflags;
}

/* Returns the string message for the last error code. */
const char * FdSelect::GetLastErrMsg() const
{
	if(LastErr == RET_OK)
		return "";
	else
		return strerror(LastErr);
}

/* Clear the list of prepared file descriptors. */
void FdSelect::Clear()
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
bool FdSelect::Add(int fd, int flags)
{
	int slot = FindFreeSlot(fd);
	if(slot == RET_ERR) return false;
	fd_list[slot].fd = fd;
	fd_list[slot].flags = flags;
	return true;
}

/* Removes a file descriptor from the list
 * of prepared fds. */
bool FdSelect::Remove(int fd)
{
	int slot = FindFd(fd);
	if(slot != RET_ERR)
		fd_list[slot].flags = 0;
	return (slot != RET_ERR);
}

/* Read the result status of the prepared fd.
 * Returns the status, or 0 if the fd was
 * not selected/updated on Wait(). */
int FdSelect::GetStatus(int fd)
{
	int slot = FindFd(fd);
	if(slot == RET_ERR) return 0;
	int iflags = fd_list[slot].flags;
	return GetStatus(fd, iflags, &rfds, &wfds, &efds);
}

/* Select/Wait for the list of prepared fds.
 * Use the Add() and Remove() to add and remove
 * fds to the list of prepared fds.
 * Returns the number of selected fds,
 * 0 on no fd/timeout or RET_ERR on error. */
int FdSelect::WaitAll(int timeoutMillis)
{
	int retval;
	int maxFd   = -1;
	int numRfds = 0;
	int numWfds = 0;
	int numEfds = 0;
	struct timeval tv;

	// clear the FDs
	FD_ZERO(&rfds);
	FD_ZERO(&wfds);
	FD_ZERO(&efds);

	// fill the FDs with the prepared file descriptors
	for(int n = 0; n < MAX_FD_LIST; n++)
	{
		if(fd_list[n].fd >= 0 && fd_list[n].flags > 0)
		{
			int fd = fd_list[n].fd;
			int flags = fd_list[n].flags;
			maxFd = MAX(fd, maxFd);
			if(HAS_FLAG(flags, SEL_READ))  { numRfds++; FD_SET(fd, &rfds); }
			if(HAS_FLAG(flags, SEL_WRITE)) { numWfds++; FD_SET(fd, &wfds); }
			if(HAS_FLAG(flags, SEL_ERROR)) { numEfds++; FD_SET(fd, &efds); }
		}
	}

	// prepare FDs for select
	fd_set* prfds = NULL;
	fd_set* pwfds = NULL;
	fd_set* pefds = NULL;
	if(numRfds > 0) prfds = & rfds;
	if(numWfds > 0) pwfds = & wfds;
	if(numEfds > 0) pefds = & efds;

	// prepare timeout
	if(timeoutMillis < 0) timeoutMillis = 0;
	tv.tv_usec = (timeoutMillis % 1000) * 1000;
	tv.tv_sec = (timeoutMillis / 1000);

	// reset last error
	LastErr = RET_OK;

	StdErr.printf("\nselect(%d, %p, %p, %p, %ds%dus)\n",
		maxFd + 1, prfds, pwfds, pefds, tv.tv_sec, tv.tv_usec);

	// select the descriptors
	retval = select(
		maxFd + 1, prfds, pwfds, pefds, &tv);

	// update LastErr on error
	if(retval == RET_ERR)
		LastErr = errno;

	return retval;
}

/* Wait/select state for a single file descriptor.
 * Returns the the state of the descriptor on success.
 * Returns 0 on timeout or RET_ERR on error. */
int FdSelect::Wait(int fd, int flags, int timeoutMillis)
{
	int retval;
	fd_set rfds;
	fd_set wfds;
	fd_set efds;
	fd_set* prfds = NULL;
	fd_set* pwfds = NULL;
	fd_set* pefds = NULL;
	struct timeval tv;

	// clear the FDs
	FD_ZERO(&rfds);
	FD_ZERO(&wfds);
	FD_ZERO(&efds);

	// fill the FDs
	if(flags & SEL_READ)  { FD_SET(fd, &rfds); prfds=&rfds; }
	if(flags & SEL_WRITE) { FD_SET(fd, &wfds); pwfds=&wfds; }
	if(flags & SEL_ERROR) { FD_SET(fd, &efds); pefds=&efds; }

	// prepare the timeout
	if(timeoutMillis < 0) timeoutMillis = 0;
	tv.tv_usec = (timeoutMillis % 1000) * 1000;
	tv.tv_sec = (timeoutMillis / 1000);

	StdErr.printf("\nselect(%d, %p, %p, %p, %ds%dus)\n",
		fd + 1, prfds, pwfds, pefds, tv.tv_sec, tv.tv_usec);

	// select the file descriptor
	retval = select(fd + 1, prfds, pwfds, pefds, &tv);

	// return status on success
	if(retval > 0) {
		retval = GetStatus(fd, flags, prfds, pwfds, pefds);
	}

	StdErr.printf("\nselect: result=%d\n", retval);

	return retval;
}
