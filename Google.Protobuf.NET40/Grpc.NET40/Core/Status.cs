using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.Core
{
    /// <summary>
    /// Representa um código de status de processamento, incluindo uma mensagem de detalhes.
    /// </summary>
    public struct Status
    {
        /// <summary>
        /// Nível de status.
        /// </summary>
        public readonly StatusCode StatusCode;

        /// <summary>
        /// Detalhe textual do nível de status.
        /// </summary>
        public readonly string Detail;

        /// <summary>
        /// Creates a new instance of <c>Status</c>.
        /// </summary>
        /// <param name="statusCode">Status code.</param>
        /// <param name="detail">Detail.</param>
        public Status(StatusCode statusCode, string detail)
        {
            this.StatusCode = statusCode;
            this.Detail = detail;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Grpc.Core.Status"/>.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Status(StatusCode={0}, Detail=\"{1}\")", this.StatusCode, this.Detail);
        }
    }
}
