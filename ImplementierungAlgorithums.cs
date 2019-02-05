using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{

    class Ecke
    {
        public int id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
    }

    class Program
    {
        public static int idEcke = 0;
        public static List<Ecke> a1 = new List<Ecke>();
        public static List<Ecke> a2 = new List<Ecke>();

        public static int posX;
        public static int posY;
        public static int posZ;

        public static int radius = 50;

        static void Main(string[] args)
        {
            SetPosition(0, 0, 0);
            EckeErstellen(10, 10, 10);
            EckeErstellen(10, 5, 10);
            EckeErstellen(100, 100, 100);
            EckeErstellen(20, 5, 20);
            Algorithmus1();
        }

        static void SetPosition (int x, int y, int z)
        {
            posX = x;
            posY = y;
            posZ = z;
        }

        static void EckeErstellen(int x, int y, int z)
        {
            Ecke p = new Ecke()
            {
                id = idEcke,
                x = x,
                y = y,
                z = z,
            };

            a1.Add(p);
            Console.WriteLine("länge a1: " +a1.Count);
            Console.ReadKey();
            idEcke++;
        }

        static void Algorithmus1()
        {
            Console.WriteLine("PosX: " + posX);
            Console.WriteLine("PosY: " + posY);
            Console.WriteLine("PosZ: " + posZ);
            Console.ReadKey();

            for (int i = 0; i<a1.Count; i++)
            {
                Console.WriteLine("Element: " + i + " id: " + a1[i].id);
                Console.WriteLine("Element: " + i + " x: " + a1[i].x);
                Console.WriteLine("Element: " + i + " y: " + a1[i].y);
                Console.WriteLine("Element: " + i + " z: " + a1[i].z);
                Console.ReadKey();

                if (
                    (a1[i].x <= (posX + radius)) && (a1[i].x > (posX - radius)) && 
                    (a1[i].y <= (posY + radius)) && (a1[i].y > (posY - radius)) && 
                    (a1[i].z <= (posZ + radius)) && (a1[i].z > (posZ - radius))
                   )
                {
                    a2.Add(a1[i]);
                }

                else
                {
                    for(int j = 0; j<a2.Count; j++)
                    {
                        if (a1[i].id == a2[j].id)
                        {
                            a2.Remove(a2[j]);
                        }
                    }
                }
            }
            Algorithmus2();
        }

        static void Algorithmus2()
        {
            int supercount = 151;
            Ecke highlightEcke = null;
            for (int i = 0; i < a2.Count; i++)
            {
                int count = 0;
                if (a2[i].x > 0)
                {
                    count += a2[i].x;
                }
                else
                {
                    count += (a2[i].x) * -1;
                }

                if (a2[i].y > 0)
                {
                    count += a2[i].y;
                }
                else
                {
                    count += (a2[i].y) * -1;
                }

                if (a2[i].z > 0)
                {
                    count += a2[i].z;
                }
                else
                {
                    count += (a2[i].z) * -1;
                }

                if (count < supercount)
                {
                    supercount = count;
                    highlightEcke = a2[i];
                }
            }

            if (a2.Count > 0)
            {
                Console.WriteLine(highlightEcke.id);
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
