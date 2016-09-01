using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesteStreamScanner.compiler.scanner;
using System.IO;
using System.Diagnostics;

namespace TesteStreamScanner.compiler
{
    public class ManualTokenizer : ITokenizer
    {
        public const int INITITAL_STATE = 0;
        public const int INPUT_EOF = 0;

        public const int NO_TOKEN = -1;
        public const int TOKEN_EOF = 0;
        public const int TOKEN_INVALID_CHAR = 1;

        protected Stream InputStream;
        protected int CurrentState;
        protected int NextState;
        protected int CurrentInput;
        protected StringBuilder Buffer;
        protected MemoryStream m_memoryStream;
        protected int RetToken;

        public void Init()
        {
            OnInit();
            ReadInput();
        }

        protected void ReadInput()
        {
            int read = InputStream.ReadByte();
            if (read == -1) read = INPUT_EOF;
            CurrentInput = read;
        }

        protected virtual void OnInit()
        {
            Buffer = new StringBuilder();
            CurrentState = INITITAL_STATE;
        }

        public void SetSource(string input)
        {
            m_memoryStream = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(input));
            m_memoryStream.Position = 0;
            InputStream = m_memoryStream;
        }

        public void SetSource(Stream input)
        {
            InputStream = input;
        }

        protected void ClearBuffer()
        {
            Buffer.Clear();
        }

        protected void AppendBuffer()
        {
            AppendBuffer(CurrentInput);
        }

        protected void AppendBuffer(int input)
        {
            Buffer.Append((char)input);
        }

        public TokenInfo GetToken()
        {
            TokenInfo ret = new TokenInfo();
            ret.Id = ProcessState();
            ret.Token = Buffer.ToString();
            Debug.WriteLine(ret.ToString());
            return ret;
        }

        protected void ReturnToken(int token)
        {
            RetToken = token;
        }

        protected int ProcessState()
        {
            while (true)
            {
                RetToken = NO_TOKEN;
                NextState = CurrentState;
                int ret = OnProcessState();
                CurrentState = NextState;
                if (RetToken != NO_TOKEN) return RetToken;
                if (ret != NO_TOKEN) return ret;
            }
        }

        protected virtual int OnProcessState()
        {
            return TOKEN_EOF;
        }
    }
}
