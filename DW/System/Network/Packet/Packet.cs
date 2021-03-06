﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace DW
{

    [Serializable] // Pour que la classe soit sérialisable
    public class Packet //Une superclasse pour les paquets
    {

        public Packet()
        {
        }

        public virtual void processPacket(PacketManager par1)
        {

        }

        //Méthode statique pour l'envoi et la réception
        public static void Send(Packet paquet, UdpClient link, IPEndPoint ie)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, paquet);
            byte[] dtmp = ms.ToArray();
            // Console.WriteLine("before " + dtmp.Length);
            byte[] dgram = Zip.Compress(dtmp);
            //Console.WriteLine("after " + dgram.Length);
            if (dgram.Length > 65507)
            {
                Console.WriteLine("Packet trop volumineux");
            }
            else
                link.Send(dgram, dgram.Length, ie);
        }

        public static Packet Receive(UdpClient link)
        {
            IPEndPoint i = null;
            byte[] dtmp;
            try
            {
                dtmp = link.Receive(ref i);
            }
            catch (SocketException)
            {
                DW.close(DW.server);
                DW.close(DW.client);
                DW.changeScene("GameMenu", "Impossible de joindre l'hote distant");
                return null;
            }

            byte[] tmp = Zip.Decompress(dtmp);
            try
            {
                // convert byte array to memory stream
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream(tmp);

                // create new BinaryFormatter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
                            = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                // set memory stream position to starting point
                _MemoryStream.Position = 0;


                return (Packet)_BinaryFormatter.Deserialize(_MemoryStream);
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine(_Exception.ToString());
            }
            return null;
        }
    }
}