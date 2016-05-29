using System;
using System.Net;
using System.Net.Sockets;
using ArtNet.Sockets;
using ArtNet.Packets;
using System.Timers;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetVLC
{
    class MainClass
    {
    	static bool started = false;
    	static int command = 0;
    	static byte[] _dmxData = new byte[511];
    	static TcpClient vlcremote = new TcpClient("127.0.0.1",8888);
    	static string localip;
    	static int dmxstartaddr;
    	static int lastvol;
    	
        public static void Main(string[] args)
        {
        	Console.WriteLine("Starte ArtNet -> VLC Schnittstelle...");
            var artnet = new ArtNet.Sockets.ArtNetSocket();
            artnet.EnableBroadcast = true;
            Console.WriteLine("Öffne config.txt");
        	try
        	{
            String[] config =File.ReadAllLines("config.txt");
        	localip = config[0];
        	dmxstartaddr = Convert.ToInt16(config[1]);
        	}
        	catch(Exception e)
        	{
        		Console.WriteLine("Fehler beim Öffnen der Konfigurationsdatei. Beende...");
        		Console.ReadLine();
        		Environment.Exit(1);
        	}
        	
            artnet.Open(IPAddress.Parse(localip), IPAddress.Parse("255.255.255.0"));
            Console.WriteLine("Höre auf IPAdresse: " + artnet.LocalIP.ToString());
            Console.WriteLine("ARTNET-Startadresse: " + dmxstartaddr.ToString());
            Console.WriteLine("Verbinde mit VLC RemoteInterface...");
            if(vlcremote.Connected == true)
            {
            	Console.WriteLine("Verbunden!");
            }

            //Neues Paket? Reagieren!
            artnet.NewPacket += (object sender, ArtNet.Sockets.NewPacketEventArgs<ArtNet.Packets.ArtNetPacket> e) => 
            {
            	//Feststellen ob ArtNet-Paket
                if(e.Packet.OpCode == ArtNet.Enums.ArtNetOpCodes.Dmx)
                { 
					//Enthaltene Daten in Puffer verschieben                	
                    var packet = e.Packet as ArtNet.Packets.ArtNetDmxPacket;
                    if(packet.DmxData != _dmxData)
                    {
                    	new_dmx();
                    	_dmxData = packet.DmxData;
                    }
                }
                    
            };
            Console.WriteLine("Warte auf Befehle...");
            while(true)
            {
            Console.ReadLine();
            }
			
        }
        
        static void new_dmx()
        {	
        	int dmxgo = Convert.ToInt16(_dmxData[dmxstartaddr]);
        	int dmxfile = Convert.ToInt16(_dmxData[dmxstartaddr+1]);
        	int dmxaudiovol = Convert.ToInt16(_dmxData[dmxstartaddr+2]);
        	//dmxaudiovol = dmxaudiovol * 2;
        	StreamWriter sr = new StreamWriter(vlcremote.GetStream(),Encoding.ASCII);
        	if (dmxaudiovol != lastvol)
        	{
        	Console.WriteLine(DateTime.Now.ToString() + " Command: Volume: " + dmxaudiovol.ToString());
        	sr.WriteLine("volume " + dmxaudiovol.ToString());
        	sr.Flush();
        	lastvol = dmxaudiovol;
        	}
        	
        	for(int i=0;i<255;i++)
        	{
        		if(dmxfile == i && dmxgo == 1)
        		{
        			if(started == false)
        			{
        			
        			Console.WriteLine(DateTime.Now.ToString() + " Command: Stop");
        			sr.WriteLine("stop");
        			sr.Flush();
        			Console.WriteLine(DateTime.Now.ToString() + " Command: goto " + dmxfile.ToString());
        			sr.WriteLine("goto " + dmxfile.ToString());
        			sr.Flush();
        			Console.WriteLine(DateTime.Now.ToString() + " Command: play");
        			sr.WriteLine("play");
        			sr.Flush();
        			started = true;
        			command = i;
        			}
        		}
        		if(dmxgo == 0 && started == true)
        		{
        			Console.WriteLine(DateTime.Now.ToString() +" Command: Stop");
        			sr.WriteLine("stop");
        			sr.Flush();
        			started = false;
        		}
        		
        		
        	}
        	
        	
        	/*Ich liebe dich Mausili!! jetzt ist es ein programmkommentar und kann stehen bleiben OMG du hast mich in dein Programm geschrieben????
	#na klar du bist mir doch unendlich wichtig :))
	#du kannst es gerade nicht sehen aber ich weine vor glück :-*
	#oh spatzili nicht weinen :)) das ertrage ich nicht :* ok aber du darfst nie aufhören sie romantische dinge für mich zu machen
	#das findest du romantisch? nai cht mir das ja ganz einfach :Ddann fäll
	#du könntest nicht romantischer sein :-*
	#die proben unten und ich hab nix zu tun da hab ich gedachrt probier ich doch mal ob das funktioniert
	#du bist so weit weg aber eigentlich bist du mir doch soooo nah. und lässt mich an deinen PC das bedeutet mir so unendlich viel 
	#das ist schön dass ich dich so "einfach" glücklich machen kann :)
	#wenn du wüsstest
	#ich freu mich drauf dich wieder zu sehen später
	#ich freu mich mindestens genau so sehr. mein herz :* dann kannst du ja jetzt von KA aus an deinem Programm schreiben oder??
	#ja ich muss noch ein paar einstellungen ändern weil wegen dem langsamen internet hier der bildschirm 2 3 sekunden braucht um 
	#ein neues bild darzustellen aber prinzipiell geht das
	#dann lass ich dich mal weiter arbeiten. ICH LIEBE DICH UNENDLICH MEIN ENGEL. wir sehen
	 #uns später. und PS den ersten satz bitte stehen lassen der rest kann weg wenn du willst
	 #ich kann auch alles stehen lassen das stört niemanden :D
	 #OK :-*
	 #okay mausili dann programmier ich mal ein bisschen... ich liebe dich!!! :* :-*****
	*/
        }
        
    }
    
}
	 