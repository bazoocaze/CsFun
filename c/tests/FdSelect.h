/* File.....: FdSelect.h
 * Author...: Jose Ferreira
 * Date.....: 2016-11-24 14:15
 * Purpose..: File descriptor status selector
 * */


#pragma once


#include <sys/select.h>


/* File descriptor status enumeration. */
enum SelFlagEnum {
	/* Select fd for reading. */
	SEL_READ  = 4,

	/* Select fd for wrting. */
	SEL_WRITE = 2,

	/* Select fd for error condition. */
	SEL_ERROR = 1
};


typedef struct {
	int fd;
	int flags;
} select_fd_t;


/* Represents a file descriptor selector
 * that can select/wait/update the status
 * of multiple fds at once. */
class FdSelect
{
protected:
	static const int MAX_FD_LIST = 64;
	select_fd_t fd_list[MAX_FD_LIST];
	int LastErr;
	fd_set rfds;
	fd_set wfds;
	fd_set efds;

	int FindFreeSlot(int fd);
	int FindFd(int fd);

	/* Extract the updated state of a single fd from the fd sets. */
	static int GetStatus(int fd, int iflags, fd_set* prfds, fd_set* pwfds, fd_set* pefds);

public:

	/* Default constructor for an empty file descriptor selector. */
	FdSelect();

	/* Returns the string message for the last error code. */
	const char * GetLastErrMsg() const;

	/* Clear the list of prepared file descriptors. */
	void Clear();

	/* Prepared and add a file descriptor to the list
	 * of prepared fds.
	 * Flags can be SEL_READ, SEL_WRITE and SEL_ERROR.
	 * Returns true on success, or false on out of free
	 * fd slots (see the MAX_FD_LIST constant).
	 * Update fd status using Wait(), and read updated
	 * status using GetStatus(). */
	bool Add(int fd, int flags);

	/* Removes a file descriptor from the list
	 * of prepared fds. */
	bool Remove(int fd);

	/* Read the result status of the prepared fd.
	 * Returns the status, or 0 if the fd was
	 * not selected/updated on Wait(). */
	int GetStatus(int fd);

	/* Select/Wait for the list of prepared fds.
	 * Use the Add() and Remove() to add and remove
	 * fds to the list of prepared fds.
	 * Returns the number of selected fds,
	 * 0 on no fd/timeout or RET_ERR on error. */
	int WaitAll(int timeoutMillis);

	/* Wait/select state for a single file descriptor.
	 * Returns the the state of the descriptor on success.
	 * Returns 0 on timeout or RET_ERR on error. */
	static int Wait(int fd, int flags, int timeoutMillis);
};
