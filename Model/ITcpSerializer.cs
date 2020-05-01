using System;
using System.Collections.Generic;
using System.IO;

namespace OnlineTableGamePlayer.Model
{
    public interface ITcpSerializer<TMessage>
    {
        void Serialize(Stream stream, TMessage message);

        TMessage Deserialize(Stream stream);
    }
}
