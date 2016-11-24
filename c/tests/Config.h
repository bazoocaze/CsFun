/*
 * Arquivo....: Config.h
 * Autor......: José Ferreira - olvebra
 * Data.......: 06/08/2015 - 14:47
 * Objetivo...: Configurações do programa.
 */


#pragma once


// #include "Util.h"


#define P_BUFFER_START_SIZE        256
#define P_DEFAULT_LOG_LEVEL        LEVEL_VERBOSE
#define P_NETWORK_READ_SIZE        1024
#define P_NETWORK_WRITE_SIZE       128
#define P_NETWORK_READ_TIMEOUT     2000


#define HAVE_CONSOLE
#define HAVE_DNS_getaddrinfo
#define HAVE_THREAD_pthreads

