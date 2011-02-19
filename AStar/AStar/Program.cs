using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    class Program
    {

        /* List of all cities in the file */
        public static List<City> allCities = new List<City>();

        /* Output File */
        public static TextWriter TR_output = File.CreateText("output.txt");

        static void Main(string[] args)
        {
            /* Read in Files */
            TextReader TR_locations = File.OpenText("locations.txt");
            TextReader TR_connections = File.OpenText("connections.txt");


            TR_output.WriteLine("Welcome to the Solver");

            /* Get Locations */
            getLocations(TR_locations);

            /* Get Connections */
            getConnections(TR_connections);

            /* Initialize Best City */
            City Best = new City();

            //Read in files
            // Create List Array[] for cities
            //Read In: Start, End, Exclude

            // while(Best != end && ListPossible != empty){
                    //Best <- ComparePossible
                    //RemoveFromPossible(Best)
                    //List <- ListPossible(Best)
                    //Update Best.previousCity
                    //Update Best.prevVisited
            //}

            TR_output.Close();
           
        }

        /***************************************
        * Function Name: getLocations
        * Pre-Conditions: StreamReader locations
        * Post-Condition: void
        * 
        * Reads the file from StreamReader and
        * add them locations to allCities
        * *************************************/
        public static void getLocations(TextReader locations)
        {
            TR_output.WriteLine("Reading in locations...");
            string input = null;
            while ((input = locations.ReadLine()) != null && input != "END")
            {
                int j = 0;
                int lastspace = -1;
                int nextspace = 0;
                City tempCity = new City();
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i].CompareTo(' ') == 0)
                    {
                        lastspace = i;
                        if (j == 0)
                        {
                            tempCity.Name = input.Substring(nextspace, lastspace);
                            nextspace = lastspace + 1;
                        }
                        else if (j == 1)
                        {

                            try
                            {
                                tempCity.x = Convert.ToInt32(input.Substring(nextspace, lastspace - nextspace));
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Input string is not a sequence of digits.");
                            }
                            catch (OverflowException e)
                            {
                                Console.WriteLine("The number cannot fit in an Int32.");
                            }

                            nextspace = lastspace + 1;
                        }
                        j++;
                    }
                }

                try
                {
                    tempCity.y = Convert.ToInt32(input.Substring(nextspace, (input.Length - nextspace)));
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Input string is not a sequence of digits.");
                }
                catch (OverflowException e)
                {
                    Console.WriteLine("The number cannot fit in an Int32.");
                }

                // Print City to the Log
                TR_output.WriteLine(tempCity.Name + " " + tempCity.x + " " + tempCity.y);

                // Add the city to the list
                allCities.Add(tempCity);
                
            }
        }

        /***************************************
         * Function Name: getLocations
         * Pre-Conditions: StreamReader locations
         * Post-Condition: void
         * 
         * Reads the file from StreamReader and
         * add them locations to allCities
         * *************************************/
        public static void getConnections(TextReader connections)
        {
            TR_output.WriteLine("Reading in connections...");
            string input = null;
            while ((input = connections.ReadLine()) != null && input != "END")
            {
                int j = 0;
                int lastspace = -1;
                int nextspace = 0;
                int place = 0;
                int totConnections = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i].CompareTo(' ') == 0)
                    {
                        lastspace = i;
                        if (j == 0)
                        {
                            string temp = input.Substring(nextspace, lastspace);

                            for (int k = 0; k < allCities.Count; k++)
                            {
                                if (allCities[k].Name.CompareTo(temp) == 0)
                                {
                                    place = k;
                                }
                            }
                            TR_output.Write(allCities[place].Name + " ");
                        }
                        else if (j == 1)
                        {
                            totConnections = Convert.ToInt32(input.Substring(nextspace, lastspace-nextspace));
                            TR_output.Write(totConnections + " ");
                        }
                        else if( j < totConnections + 2)
                        {
                            string temp = input.Substring(nextspace, lastspace - nextspace);

                            int tempPlace = 0;
                            for (int k = 0; k < allCities.Count; k++)
                            {
                                if (allCities[k].Name.CompareTo(temp) == 0)
                                {
                                    tempPlace = k;
                                }
                            }
                            //allCities[place].connections.Add(allCities[tempPlace]);
                            // allCities[place].connections.Add(allCities[tempPlace]);
                            City tempCity = allCities[tempPlace];
                            allCities[place].addConnection(tempCity);
                            TR_output.Write(tempCity.Name + " ");
                        }
                        nextspace = lastspace + 1;
                        j++;
                    }
                }
                string temp2 = input.Substring(nextspace, input.Length - nextspace);

                int tempPlace2 = 0;
                for (int k = 0; k < allCities.Count; k++)
                {
                    if (allCities[k].Name.CompareTo(temp2) == 0)
                    {
                        tempPlace2 = k;
                    }
                }
                City tempCity2 = allCities[tempPlace2];
                allCities[place].addConnection(tempCity2);
                TR_output.WriteLine(tempCity2.Name);
            }
        }

        /***************************************
        * Function Name: Distance
        * Pre-Conditions: City A, City B
        * Post-Condition: Double
        * 
        * Return the numerical distance between
        * City A and City B as a double
        * *************************************/
        public double Distance(City A, City B)
        {
            return Math.Sqrt((B.x - A.x) ^ 2 + (B.y - A.y) ^ 2);
        }

        /***************************************
         * Function Name: ComparePossibilities
         * Pre-Conditions: List<City> Possible
         *                 int Distance
         * Post-Condition: City
         * 
         * Determines the best possible city for the
         * A* algorithm to choose. Removes that city
         * from the list, and returns the best city
         * *************************************/
        public City ComparePossibilities(List<City> Possible, int distance)
        {
            return new City();
        }

        /***************************************
         * Function Name: Calculate Estimate
         * Pre-Conditions: City theCity
         *                 int Distance
         * Post-Condition: int
         * 
         * Calculates the Estimate between the passed
         * City and the 
         * *************************************/
        public int CalculateEstimate(City theCity, int distance)
        {
            return 1;
        }
    }
}
