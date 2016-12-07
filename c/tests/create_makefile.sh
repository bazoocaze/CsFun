#!/bin/bash


NEWFILE="Makefile.new"
OLDFILE="Makefile"

SRCC=$(find . -name "*.c" | sed 's|[.]/||g;')
SRCCXX=$(find . -name "*.cpp" | sed 's|[.]/||g;')


CLEAROUT()
{
echo "# Auto-generated on $(date -R)" >${NEWFILE}
OUT ""
}


OUT()
{
echo "$@" >>${NEWFILE}
}


READVAR()
{
local varname="$1"
local defvalue="$2"
local val
	val=$(cat ${OLDFILE} 2>/dev/null | sed -n "s|${varname} *:= *\(.*\)|\1|p;")
	if [ -n "${val}" ] ; then
		echo -n "${val}"
	else
		echo -n "${defvalue}"
	fi
}


MODVAR()
{
local varname="$1"
local defvalue="$2"
	OUT "${varname} := "$(READVAR "${varname}" "${defvalue}")
}


OUTVAR()
{
local varname="$1"
local defvalue="$2"
	OUT "${varname} := ${defvalue}"
}



PROJECT=$(READVAR "PROJECT" "teste")
TARGETS=$(READVAR "TARGETS" "teste")


CLEAROUT

[ -n "${SRCC}" ]   && MODVAR "CFLAGS"   "-g -Wall -Wextra"
[ -n "${SRCCXX}" ] && MODVAR "CXXFLAGS" "-g -Wall -Wextra --std=c++11"

OBJS_C=$(echo ${SRCC} | sed 's|\.c|.o|g;') 
OBJS_CXX=$(echo ${SRCCXX} | sed 's|\.c..|.o|g;') 
OBJS="${OBJS_C} ${OBJS_CXX}"
DEPS=$(echo ${OBJS} | sed 's|\.o|.dep|g;')

# modifiable
MODVAR "LDLIBS"  ""
MODVAR "PROJECT" "${PROJECT}"
MODVAR "TARGETS" "${TARGETS}"

# fixed
OUT ""
OUTVAR "OBJS" "${OBJS}"
OUTVAR "DEPS" "${DEPS}"

# if g++ project then link with g++
[ -n "${SRCCXX}" ] && OUT 'LINK.o = $(LINK.cpp)'

OUT ''
OUT 'all: $(TARGETS)'
OUT ''
OUT 'redo: clean all'
OUT ''
OUT '$(PROJECT): $(OBJS)'
OUT '	$(LINK.o) $^ $(LOADLIBES) $(LDLIBS) -o $@'
OUT ''
OUT '$(PROJECT).a: $(OBJS)'
OUT '	$(AR) $(ARFLAGS) $@ $<'
OUT ''
OUT '%.dep: %.cpp'
OUT '	$(CXX) -MM $< -o $@'
OUT ''
OUT '%.dep: %.c'
OUT '	$(CC) -MM $< -o $@'
OUT ''
OUT 'clean:'
OUT '	rm -f $(TARGETS) *.o *.a *.dep'
OUT ''
OUT 'check:'
OUT '	cppcheck -v --enable=all .'
OUT ''
OUT '-include $(DEPS)'
OUT ''


cat ${NEWFILE}

mv -f "${OLDFILE}" "${OLDFILE}.old"
mv -f "${NEWFILE}" "${OLDFILE}"

## colordiff "${OLDFILE}.old" "${OLDFILE}"
