using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace invertedList
{
    class invertedlist
    {
        List<String[]> listOfRec = new List<string[]>();
        SortedDictionary<String, int> listOfPK = new SortedDictionary<String, int>();
        Dictionary<string, Dictionary<int, string>> invertedList = new Dictionary<string, Dictionary<int, string>>();
        SortedDictionary<int, string> listOfreferences = new SortedDictionary<int, string>();

        String filename;
        int keyindex;

        public void implement()
        {
            while (true)
            {
                while (filename == null)
                {
                    Console.WriteLine("Choose test case (1 / 2 / 3 / 4 / 5)");
                    int ch = int.Parse(Console.ReadLine());
                    switch (ch)
                    {
                        case 1:
                            filename = "record1.txt";
                            break;
                        case 2:
                            filename = "record2.txt";
                            break;
                        case 3:
                            filename = "record3.txt";
                            break;
                        case 4:
                            filename = "record4.txt";
                            break;
                        case 5:
                            filename = "record5.txt";
                            break;
                        default:
                            Console.WriteLine("wrong choice!");
                            break;
                    }
                }

                Console.WriteLine("Enter secondary key you want to display with : (name / city / department / grade)");
                string sec = Console.ReadLine();
                if (sec.Equals("name"))
                {
                    keyindex = 2;
                    break;
                }
                else if (sec.Equals("city"))
                {
                    keyindex = 3;
                    break;
                }
                else if (sec.Equals("department"))
                {
                    keyindex = 4;
                    break;
                }
                else if (sec.Equals("grade"))
                {
                    keyindex = 5;
                    break;
                }
                else
                    Console.WriteLine("invalid input, please try again!");
            }



            try
            {
                //loading records from file
                FileStream st = new FileStream(filename, FileMode.OpenOrCreate);
                StreamReader sr = new StreamReader(st);
                string line;
                string[] lineArr;

                while ((line = sr.ReadLine()) != null)
                {
                    lineArr = line.Split("\t");
                    listOfRec.Add(lineArr);
                }
                sr.Close();

            }
            catch { Console.WriteLine("error opening file"); };

            for (int i = 0; i < listOfRec.Count; i++) //primary key table
            {
                listOfPK.Add(listOfRec.ElementAt(i)[1], int.Parse(listOfRec.ElementAt(i)[0]));
            }


            List<string> secKeys = new List<string>();
            for (int i = 0; i < listOfRec.Count; i++)  //secondary key table
            {
                if (!secKeys.Contains(listOfRec.ElementAt(i)[keyindex]))
                {
                    secKeys.Add(listOfRec.ElementAt(i)[keyindex]);
                }
            }
            secKeys.Sort();

            Dictionary<int, string> references = new Dictionary<int, string>();
            for (int i = 0; i < secKeys.Count; i++)
            {
                references = new Dictionary<int, string>();
                for (int j = 0; j < listOfRec.Count; j++)
                {
                    //collect all the records that have this secondary key
                    if (listOfRec.ElementAt(j)[keyindex].Equals(secKeys.ElementAt(i)))
                    {
                        references.Add(int.Parse(listOfRec.ElementAt(j)[0]), listOfRec.ElementAt(j)[1]);
                    }
                }
                //put this secondary key and its records as element in the inverted list
                invertedList.Add(secKeys.ElementAt(i), references);
            }

            //List of references
            listOfreferences = new SortedDictionary<int, string>();
            for (int i = 0; i < invertedList.Count; i++)
            {
                for (int j = 0; j < invertedList.ElementAt(i).Value.Count; j++)
                {
                    if (j == invertedList.ElementAt(i).Value.Count - 1) //last element in the list of this secondary key
                        listOfreferences.Add(invertedList.ElementAt(i).Value.ElementAt(j).Key, invertedList.ElementAt(i).Value.ElementAt(j).Value + "\t\t" + -1);
                    else
                        listOfreferences.Add(invertedList.ElementAt(i).Value.ElementAt(j).Key, invertedList.ElementAt(i).Value.ElementAt(j).Value + "\t\t" + invertedList.ElementAt(i).Value.ElementAt(j + 1).Key.ToString());

                }
            }

            display();

            Console.WriteLine("Do you want to add new record? (y / n)");
            char input = char.Parse(Console.ReadLine());
            if (input.Equals('y'))
            {
                Console.Write("id: ");
                string id = Console.ReadLine();
                Console.Write("Name: ");
                string name = Console.ReadLine();
                Console.Write("city: ");
                string city = Console.ReadLine();
                Console.Write("department: ");
                string dep = Console.ReadLine();
                Console.Write("Grade: ");
                string grade = Console.ReadLine();

                Stopwatch wat = Stopwatch.StartNew();
                addUpdate(id, name, city, dep, grade);
                wat.Stop();
                display();
                Console.WriteLine();
                Console.WriteLine("Time taken: {0}ms", wat.Elapsed.TotalMilliseconds);
            }

        }


        public void display()
        {
            Console.WriteLine("________________________________");
            //Display primary key table
            Console.WriteLine("primary key" + "\t" + "Ref");
            for (int i = 0; i < listOfPK.Count; i++)
            {
                Console.WriteLine(listOfPK.ElementAt(i).Key + "\t\t" + listOfPK.ElementAt(i).Value);
            }

            //display references table
            Console.WriteLine("________________________________________");
            Console.WriteLine("Sec key" + "\t\t" + "Ref");
            for (int i = 0; i < invertedList.Count; i++)
            {
                Console.WriteLine(String.Format("{0,-15}  {1,-15} ", invertedList.ElementAt(i).Key, invertedList.ElementAt(i).Value.ElementAt(0).Key));
            }

            //display inverted list table
            Console.WriteLine("____________________________________________");
            for (int i = 0; i < listOfreferences.Count; i++)
            {
                Console.WriteLine(listOfreferences.ElementAt(i).Key + "\t\t" + listOfreferences.ElementAt(i).Value);
            }

            //display records table
            Console.WriteLine("____________________________________________");
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
            StreamReader r = new StreamReader(fs);
            string ln;

            while ((ln = r.ReadLine()) != null)
            {
                Console.WriteLine(ln);
            }
            r.Close();
            fs.Close();

        }

        public void addUpdate(String id, String name, String city, String dep, String grade)
        {
            
            FileStream st = new FileStream(filename, FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(st);
            int index = 0;
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                index++;   //get number of lines to make it as reference to the added line
            }
            sr.Close();
            st.Close();
            //write to file
            FileStream s = new FileStream(filename, FileMode.Append);
            StreamWriter sw = new StreamWriter(s);
            sw.WriteLine(index.ToString() + "\t" + id + "\t" + name + "\t" + city + "\t" + dep + "\t" + grade);
            //index++;
            sw.Close();
            s.Close();

            //update tables
            String[] a = { index.ToString(), id, name, city, dep, grade };
            listOfRec.Add(a);
            listOfPK.Add(id, index);

            String seckey = "";
            switch (keyindex)
            {
                case 2:
                    seckey = name;
                    break;
                case 3:
                    seckey = city;
                    break;
                case 4:
                    seckey = dep;
                    break;
                case 5:
                    seckey = grade;
                    break;
            }

            if (!invertedList.ContainsKey(seckey))
            {
                Dictionary<int, String> dic = new Dictionary<int, string>();
                dic.Add(index, id);
                invertedList.Add(seckey, dic);
            }
            else
            {
                for (int i = 0; i < invertedList.Count; i++)
                {
                    if (invertedList.ElementAt(i).Key.Equals(seckey))
                    {
                        int key = 0;
                        String value = "";
                        for (int j = 0; j < listOfRec.Count; j++)
                        {
                            if (int.Parse(listOfRec.ElementAt(j)[0]) == invertedList.ElementAt(i).Value.ElementAt(invertedList.ElementAt(i).Value.Count - 1).Key)
                            {
                                key = int.Parse(listOfRec.ElementAt(j)[0]);
                                value = listOfRec.ElementAt(j)[1];
                            }
                        }
                        invertedList.ElementAt(i).Value.Remove(invertedList.ElementAt(i).Value.ElementAt(invertedList.ElementAt(i).Value.Count - 1).Key);
                        invertedList.ElementAt(i).Value.Add(key, value);
                        invertedList.ElementAt(i).Value.Add(index, id);
                    }

                }
            }

            //update List of references
            listOfreferences = new SortedDictionary<int, string>();
            for (int i = 0; i < invertedList.Count; i++)
            {
                for (int j = 0; j < invertedList.ElementAt(i).Value.Count; j++)
                {
                    if (j == invertedList.ElementAt(i).Value.Count - 1) //last element in the list of this secondary key
                        listOfreferences.Add(invertedList.ElementAt(i).Value.ElementAt(j).Key, invertedList.ElementAt(i).Value.ElementAt(j).Value + "\t\t" + -1);
                    else
                        listOfreferences.Add(invertedList.ElementAt(i).Value.ElementAt(j).Key, invertedList.ElementAt(i).Value.ElementAt(j).Value + "\t\t" + invertedList.ElementAt(i).Value.ElementAt(j + 1).Key.ToString());

                }
            }

            
        }

    }
}
