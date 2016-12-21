/*
 * File.....: Config.h
 * Author...: Jose Ferreira
 * Date.....: 2015-08-06 - 14:47
 * Purpose..: Global configurations.
 */


#pragma once


#define P_BUFFER_START_SIZE        256
#define P_DEFAULT_LOG_LEVEL        LEVEL_VERBOSE
#define P_NETWORK_READ_SIZE        1024
#define P_NETWORK_WRITE_SIZE       128
#define P_NETWORK_READ_TIMEOUT     2000
#define P_NETWORK_SOCK_BACKLOG     1


#define P_DNS_MAX_IPS                     16
#define P_BYTEBUFFER_MAX_SIZE             (1024*1024*16)
#define P_PROTOBUF_DEF_MAX_STR_SIZE       1024


#define HAVE_CONSOLE                      1
#define HAVE_DNS_getaddrinfo              1
#define HAVE_THREAD_pthreads              1

