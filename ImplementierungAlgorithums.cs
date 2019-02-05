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
            SetPosition(100, 90, 90);
            Algorithmus1();
            SetPosition(200, 200, 200);
            EckeErstellen(210, 210, 210);
            SetPosition(0, 0, 0);
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
            idEcke++;
        }

        static void Algorithmus1()
        {

            for (int i = 0; i<a1.Count; i++)
            {
                if (
                    (a1[i].x <= (posX + radius)) && (a1[i].x > (posX - radius)) && 
                    (a1[i].y <= (posY + radius)) && (a1[i].y > (posY - radius)) && 
                    (a1[i].z <= (posZ + radius)) && (a1[i].z > (posZ - radius))
                   )
                {
                    int count = 0;
                    for (int j = 0; j < a2.Count; j++)
                    {
                        if (a1[i].id == a2[j].id)
                        {
                            count++;
                        }
                    }
                    if (count == 0)
                    {
                    a2.Add(a1[i]);
                    }                   
                }

                else
                {
                    for(int j = 0; j<a2.Count; j++)
                    {
                        if (a1[i].id == a2[j].id)
                        {
                            a2.RemoveAt(j);
                        }
                    }
                }
            }
            Algorithmus2();
        }

        static void Algorithmus2()
        {
            int supercount = (posX+posY+posZ)+(3*radius)+1;
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
               
            if (a2.Count > 0 && highlightEcke!=null)
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
