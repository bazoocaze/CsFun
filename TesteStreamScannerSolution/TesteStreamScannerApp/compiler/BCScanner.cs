using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteStreamScanner.compiler
{
    public class BCScanner : ManualTokenizer
    {
        public const int STATE_IDENT = 10;
        public const int STATE_NUMBER = 11;

        public const int TOKEN_IDENT = 10;
        public const int TOKEN_NUMBER = 11;
        public const int TOKEN_PLUS = 12;

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override int OnProcessState()
        {
            switch (CurrentState)
            {
                case INITITAL_STATE:
                    if ((CurrentInput >= 'a' && CurrentInput <= 'z') || (CurrentInput >= 'A' && CurrentInput <= 'Z'))
                    {
                        NextState = STATE_IDENT; ClearBuffer(); AppendBuffer(); ReadInput();
                    }
                    else if (CurrentInput >= '0' && CurrentInput <= '9')
                    {
                        NextState = STATE_NUMBER; ClearBuffer(); AppendBuffer(); ReadInput();
                    }
                    else if (CurrentInput == ' ' || CurrentInput == '\t' || CurrentInput == '\n')
                    {
                        ReadInput();
                    }
                    else if (CurrentInput == '+')
                    {
                        ClearBuffer(); AppendBuffer(); ReadInput(); return TOKEN_PLUS;
                    }
                    else if (CurrentInput == INPUT_EOF)
                    {
                        ClearBuffer(); return TOKEN_EOF;
                    }
                    else
                    {
                        ClearBuffer(); AppendBuffer(); ReadInput(); return TOKEN_INVALID_CHAR;
                    }
                    break;
                case STATE_IDENT:
                    if ((CurrentInput >= 'a' && CurrentInput <= 'z') || (CurrentInput >= 'A' && CurrentInput <= 'Z') || (CurrentInput >= '0' && CurrentInput <= '9'))
                    {
                        AppendBuffer(); ReadInput();
                    }
                    else
                    {
                        NextState = INITITAL_STATE; return TOKEN_IDENT;
                    }
                    break;
                case STATE_NUMBER:
                    if (CurrentInput >= '0' && CurrentInput <= '9')
                    {
                        AppendBuffer(); ReadInput();
                    }
                    else
                    {
                        NextState = INITITAL_STATE; return TOKEN_NUMBER;
                    }
                    break;
            }
            return NO_TOKEN;
        }
    }
}
