using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Grpc.Core.Internal
{
    /// <summary>
    /// Razão de desconexão (do transporte de rede).
    /// </summary>
    public class DisconnectReason
    {
        /// <summary>
        /// True/false se o transporte foi desconectado por causa de um fim de fluxo (EOF/EOS).
        /// </summary>
        public bool Eof { get; protected set; }

        /// <summary>
        /// Exception caso o transporte tenha sido desconectado por uma condição de erro.
        /// </summary>
        public Exception Exception { get; protected set; }

        /// <summary>
        /// Construtor padrão para uma desconexão por EOF.
        /// </summary>
        public DisconnectReason()
        {
            this.Eof = true;
        }

        /// <summary>
        /// Construtor padrão para uma desconexão por uma condição de erro.
        /// </summary>
        /// <param name="ex">Exception causadora da desconexão.</param>
        public DisconnectReason(Exception ex)
        {
            this.Exception = ex;
        }

        /// <summary>
        /// Retorna uma exception padrão para a desconexão.
        /// Em caso de desconexão por EOF, retorna EndOfStreamException.
        /// </summary>
        /// <returns>Retorna uma exception padrão para a desconexão.</returns>
        public Exception GetException()
        {
            if (this.Exception != null) return this.Exception;
            if (Eof) return new EndOfStreamException();
            return new Exception("Desconectado (razão desconhecida)");
        }
    }
}
