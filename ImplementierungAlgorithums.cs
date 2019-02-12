using System;
using System.Collections.Generic;

namespace ConsoleApp2
{

    class Ecke
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
    }

    class Kante
    {
        public Ecke p1 { get; set; }
        public Ecke p2 { get; set; }
    }

    class Program
    {
        public static List<Ecke> a1 = new List<Ecke>();
        public static List<Ecke> a2 = new List<Ecke>();
        public static List<Kante> a3 = new List<Kante>();

        public static int posX;
        public static int posY;
        public static int posZ;

        public static int radius = 50;

        static void Main(string[] args)
        {
            SetPosition(-1, -1, -1);
            EckeErstellen(1, 1, 2);
            EckeErstellen(1, 1, 3);
            EckeErstellen(50, 50, 50);
            EckeErstellen(150, 100, 100);
            Algorithmus1();
            SetPosition(100, 100, 100);
            Algorithmus1();
            SetPosition(200, 200, 200);
            EckeErstellen(210, 210, 210);
            SetPosition(0, 0, 0);
            Algorithmus1();
            SetPosition(220, 220, 220);
            Algorithmus1();
        }

        static void SetPosition(int x, int y, int z)
        {
            posX = x;
            posY = y;
            posZ = z;
        }

        static void EckeErstellen(int x, int y, int z)
        {
            Ecke p = new Ecke()
            {
                x = x,
                y = y,
                z = z,
            };

            a1.Add(p);
        }

        static void KanteErstellen(Ecke p1, Ecke p2)
        {
            Kante k = new Kante()
            {
                p1 = p1,
                p2 = p2
            };
            a3.Add(k);
        }

        static void EckeVerformen(Ecke p, int x, int y, int z)
        {
            p.x = x;
            p.y = y;
            p.z = z;
        }

        static void EckeLoeschen(Ecke p)
        {
            foreach(var k in a3)
            {
                if (k.p1 == p || k.p2 == p)
                {
                    a3.Remove(k);
                }
            }

            a1.Remove(p);
        }

        static void Algorithmus1()
        {

            foreach (var p in a1)
            {
                if (
                    (p.x <= (posX + radius)) && (p.x > (posX - radius)) &&
                    (p.y <= (posY + radius)) && (p.y > (posY - radius)) &&
                    (p.z <= (posZ + radius)) && (p.z > (posZ - radius))
                   )
                {
                    if (!a2.Contains(p))
                    {
                        a2.Add(p);
                    }
                }

                else
                {
                    if (a2.Contains(p))
                    {
                        a2.Remove(p);
                    }
                }
            }
            Algorithmus2();
        }

        static void Algorithmus2()
        {
            int supercount = (posX + posY + posZ) + (3 * radius) + 1;
            Ecke highlightEcke = null;

            foreach(var p in a2)
                {
                    int count = 0;
                    count += Math.Abs(p.x - posX);
                    count += Math.Abs(p.y - posY);
                    count += Math.Abs(p.z - posZ);

                    if (count < supercount)
                    {
                        supercount = count;
                        highlightEcke = p;
                    }
                }

            if (a2.Count > 0 && highlightEcke != null)
            {
                Console.WriteLine(highlightEcke.x);
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("keine Ecke");
                Console.ReadKey();
            }
        }
    }
}
