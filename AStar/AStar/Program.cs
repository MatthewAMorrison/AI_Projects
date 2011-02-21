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

        /* List of Possible Cities */
        public static List<City> possibleCities = new List<City>();

        /* Output File */
        public static TextWriter TR_output = File.CreateText("../../../../output.txt");

        /* Origin, Destination and Avoiding Cities */
        public static string origin = null;
        public static string destination = null;
        public static List<string> excludeCities = new List<string>();

        static void Main(string[] args)
        {
            /* Read in Files */
            TextReader TR_locations = File.OpenText("../../../../locations.txt");
            TextReader TR_connections = File.OpenText("../../../../connections.txt");


            TR_output.WriteLine("Welcome to the Solver");

            /* Get Locations */
            getLocations(TR_locations);

            /* Get Connections */
            getConnections(TR_connections);

            /* Print City Information to the User */
            printCityInfo();

            /* Read In: Start, End, Exclude */
            getUserInput();

            /* Initialize Possible List */
            initializePossibleList();

            /* Initialize Best City */
            City Best = new City();
            int originPlace = 0;
            for (int i = 0; i < allCities.Count; i++)
            {
                if (allCities[i].Name.CompareTo(origin) == 0)
                {
                    originPlace = i;
                }
            }
            Best = allCities[originPlace];

            /* Get Initial Estimate */
            int estimate = 0;
            for (int k = 0; k < allCities.Count; k++)
            {
                if (allCities[k].Name.CompareTo(origin) == 0)
                {
                    estimate = CalculateEstimate(allCities[k]);
                    break;
                }
            }

            /* Print Initial Estimate to the User */
            TR_output.WriteLine("Initial Estimate: " + estimate);

            /* while(Best != end && ListPossible != empty){ */
            while (Best.Name.CompareTo(destination) != 0 && possibleCities.Count != 0)
            {
                /* Best <- ComparePossible */
                City previousCity = Best;
                Best = ComparePossibilities(previousCity, estimate);
                //RemoveFromPossible(Best)
                RemoveFromPossible(Best);
                //List <- ListPossible(Best)
                Best.prevCity = previousCity;
                Best.prevVisited = true;

                //Update Best.previousCity
                //Update Best.prevVisited
                UpdatePrevious(previousCity);

                //Best.Name = destination; // This is here to break the while loop until we finish the algorithm
            }

            TR_output.WriteLine("Destination Found: " + Best.Name);

            TR_output.Close();
           
        }

        /***************************************
        * Function Name: getLocations
        * Pre-Conditions: StreamReader locations
        * Post-Condition: void
        * 
        * Reads the file from StreamReader and
        * add the locations to allCities
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
         * Function Name: getConnecntions
         * Pre-Conditions: StreamReader locations
         * Post-Condition: void
         * 
         * Reads the file from StreamReader and
         * add the connections to existing allCities
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
        * Function Name: getUserInput()
        * Pre-Conditions: void
        * Post-Condition: void
        * 
        * Reads user input into data
        * *************************************/
        public static void getUserInput()
        {
            TR_output.WriteLine("---------------------------------");
            bool originCheck = false;
            bool destinationCheck = false;
            bool excludeCheck = false;

            while (!originCheck)
            {
                TR_output.WriteLine("Input Origin City");
                Console.WriteLine("Input Origin City");
                origin = Console.ReadLine();
                TR_output.WriteLine(origin);

                for (int i = 0; i < allCities.Count; i++)
                {
                    if (allCities[i].Name.CompareTo(origin) == 0)
                    {
                        originCheck = true;
                    }
                }
                if (!originCheck)
                {
                    TR_output.WriteLine(origin + " is not a valid city");
                    Console.WriteLine(origin + " is not a valid city");
                }
            }

            while (!destinationCheck)
            {
                Console.WriteLine("Input Destination City");
                TR_output.WriteLine("Input Destination City");
                destination = Console.ReadLine();
                TR_output.WriteLine(destination);
                for (int i = 0; i < allCities.Count; i++)
                {
                    if (origin.CompareTo(destination) == 0 && allCities[i].Name.CompareTo(destination) == 0)
                    {
                        TR_output.WriteLine(destination + " is the origin city");
                        Console.WriteLine(destination + " is the origin city");
                    }
                    else if (allCities[i].Name.CompareTo(destination) == 0)
                    {
                        destinationCheck = true;
                    }
                }
                if (!destinationCheck)
                {
                    TR_output.WriteLine(destination + " is not a valid city");
                    Console.WriteLine(destination + " is not a valid city");
                }
            }

            while (!excludeCheck)
            {
                Console.WriteLine("Input Excluded Cities or END");
                TR_output.WriteLine("Input Excluded Cities or END");
                string tempExclude = Console.ReadLine();
                TR_output.WriteLine(tempExclude);

                bool validCheck = false;

                if (tempExclude.CompareTo("END") == 0)
                {
                    excludeCheck = true;
                }
                else
                {
                    for (int i = 0; i < allCities.Count; i++)
                    {
                        if (tempExclude.CompareTo(origin) == 0)
                        {
                            TR_output.WriteLine(tempExclude + " is your selected origin city");
                            Console.WriteLine(tempExclude + " is your selected origin city");
                            break;
                        }
                        else if (tempExclude.CompareTo(destination) == 0)
                        {
                            TR_output.WriteLine(tempExclude + " is your selected destination city");
                            Console.WriteLine(tempExclude + " is your selected destination city");
                            break;
                        }
                        else if (allCities[i].Name.CompareTo(tempExclude) == 0)
                        {
                            validCheck = true;
                            break;
                        }
                    }
                    if (!validCheck)
                    {
                        TR_output.WriteLine(tempExclude + " is not a valid city");
                        Console.WriteLine(tempExclude + " is not a valid city");
                    }
                    else
                    {
                        excludeCities.Add(tempExclude);
                    }
                }
            }

            TR_output.WriteLine("----------------------------------");
            TR_output.WriteLine("User Inputs:");
            TR_output.WriteLine("Origin: " + origin);
            TR_output.WriteLine("Destination: " + destination);

            TR_output.Write("Excluded Cities:");

            for (int i = 0; i < excludeCities.Count; i++)
            {
                TR_output.Write(" " + excludeCities[i]);
            }
            TR_output.WriteLine();
            TR_output.WriteLine("----------------------------------");
        }

        /***************************************
        * Function Name: initializePossibleList
        * Pre-Conditions: void
        * Post-Condition: void
        * 
        * Initializes the possible city list
        * based on user input
        * *************************************/
        public static void initializePossibleList()
        {
            TR_output.WriteLine("Initial Possible Cities: ");
            int originNumber = 0;
            
            for (int i = 0; i < allCities.Count; i++)
            {
                if (allCities[i].Name.CompareTo(origin) == 0)
                {
                    originNumber = i;
                    break;
                }
            }
            
            for (int j = 0; j < allCities[originNumber].getConnectionTotal(); j++)
            {
                bool check = true;
                for(int k = 0; k < excludeCities.Count; k++){
                    
                    if (allCities[originNumber].getConnectionName(j).CompareTo(excludeCities[k]) == 0)
                    {
                        check = false;
                        break;
                    }
                    
                }
                if(check){

                    City tempCity = allCities[originNumber].getConnection(j);
                    TR_output.WriteLine(tempCity.Name + " " + tempCity.x + " " + tempCity.y);
                    possibleCities.Add(tempCity);
                }
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
        public static double Distance(City A, City B)
        {
            return Math.Sqrt((B.x - A.x)*(B.x - A.x) + (B.y - A.y)*(B.y - A.y));
        }

        /***************************************
        * Function Name: printCityInfo
        * Pre-Conditions: City A, City B
        * Post-Condition: Double
        * 
        * Return the numerical distance between
        * City A and City B as a double
        * *************************************/
        public static void printCityInfo()
        {
            for (int i = 0; i < allCities.Count; i++)
            {
                TR_output.WriteLine("------------------------");
                TR_output.WriteLine("City Name: " + allCities[i].Name);
                TR_output.WriteLine("x: " + allCities[i].x + " y: " + allCities[i].y);
                TR_output.Write("Connections:");
                for (int j = 0; j < allCities[i].getConnectionTotal(); j++)
                {
                    TR_output.Write(" " + allCities[i].getConnectionName(j));
                }
                TR_output.WriteLine();
            }
        }

        /***************************************
         * Function Name: ComparePossibilities
         * Pre-Conditions: City Best, int Distance
         * Post-Condition: City
         * 
         * Determines the best possible city for the
         * A* algorithm to choose. Removes that city
         * from the list, and returns the best city
         * *************************************/
        public static City ComparePossibilities(City Best, int estimate)
        {
            int place = -1;
            int curEstimate = estimate;
            for (int i = 0; i < possibleCities.Count; i++)
            {
                Console.WriteLine(i);
                int tempEstimate = (int)Distance(Best, possibleCities[i]);

                /* Is not a possibility if it is greater than estimate*/
                if (tempEstimate <= curEstimate)
                {
                    curEstimate = tempEstimate;
                    place = i;
                }
            }

            if (place != -1)
            {
                return possibleCities[place];
            }
            else
            {
                return Best;
            }
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
        public static int CalculateEstimate(City theCity)
        {
            int i = 0;
            for (i = 0; i < allCities.Count; i++)
            {
                if (allCities[i].Name.CompareTo(destination) == 0)
                {
                    break;
                }
            }

            return (int)Distance(theCity, allCities[i]);
        }

        /***************************************
        * Function Name: RemoveFromPossible
        * Pre-Conditions: City Best
        * Post-Condition: void
        * 
        * Removes the Estimate from possible
        * *************************************/
        public static void RemoveFromPossible(City Best)
        {
            for (int i = 0; i < possibleCities.Count; i++)
            {
                if (Best.Name.CompareTo(possibleCities[i].Name) == 0)
                {
                    possibleCities.Remove(Best);
                    break;
                }
            }
        }

        /***
         * Function: UpdatePrevious
         * PreCondition: City Old
         * PostCondition: void
         * */
        public static void UpdatePrevious(City toUpdate)
        {
            for (int i = 0; i < allCities.Count; i++)
            {
                if (allCities[i].Name.CompareTo(toUpdate.Name) == 0)
                {
                    allCities[i].prevCity = toUpdate.prevCity;
                    allCities[i].prevVisited = toUpdate.prevVisited;
                    break;
                }
            }
        }
    }
}
