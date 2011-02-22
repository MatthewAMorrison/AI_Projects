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

        /* Final List to Print To User */
        public static List<string> finalList = new List<string>( );

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
                    allCities[i].prevVisited = true;
                    allCities[i].prevCity = null;
                    originPlace = i;
                    allCities[i].distToStart = 0;
                    
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
            while ( possibleCities.Count != 0 && Best.Name.CompareTo(destination) != 0 )
            {
                /* Best <- ComparePossible */
                City previousCity = Best;
                //RemoveFromPossible(Best)
                Console.WriteLine("Entering RemoveFromPossible:"); 
                RemoveFromPossible(previousCity);
                //Update list of possibles [while doing this, set prevCity]
                Console.WriteLine("Entering updatePossibleList:");
                updatePossibleList(previousCity.Name);
                //Choose the next city in the path
                Console.WriteLine("Entering ComparePossibilities:");
                Best = ComparePossibilities();
                //Console.WriteLine("Best is: " + Best.Name);

                //Console.WriteLine(Best.Name + " :p " + destination + " " + Best.Name.CompareTo(destination));
            }

            printPrevVisited(Best);

           

            printList(Best);



            Console.Write("Press ENTER to finish program");
            string temp = Console.ReadLine();

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
                    tempCity.prevCity = allCities[originNumber];
                    tempCity.distToStart = 0 + (int)Distance(allCities[originNumber], tempCity);
                }
            }
        }


        /***************************************
        * Function Name: updatePossibleList
        * Pre-Conditions: String cityName
        * Post-Condition: void
        * 
        * Updates the possible city list
        * based on user input
        * *************************************/
        public static void updatePossibleList(String cityName)
        {
            Console.WriteLine("Entered updatePossibleList.  Previous City was " + cityName);
            Console.ReadLine();
            TR_output.WriteLine("Update Possible Cities: ");
            int index = 0;
            City current;
            bool check = true;
            bool updated = false;

            for (int i = 0; i < allCities.Count; i++)
            {
                if (allCities[i].Name.CompareTo(cityName) == 0)
                {
                    index = i;
                    break;
                }
            }

            current = allCities[index];

            for (int j = 0; j < current.getConnectionTotal(); j++)
            {
                check = true;
                updated = false;
                for (int k = 0; k < excludeCities.Count; k++)
                {

                    if (current.getConnectionName(j).CompareTo(excludeCities[k]) == 0)
                    {
                        check = false;
                        break;
                    }

                }

               
                City tempCity = current.getConnection(j);

                Console.WriteLine("Attempting to add " + tempCity.Name);
                Console.WriteLine(tempCity.Name + " prevVisited == " + tempCity.prevVisited);
                Console.ReadLine();
                if (check && !tempCity.prevVisited)
                {
                    Console.WriteLine(tempCity.Name + " is not excluded and not previously visited.");
                    Console.ReadLine();

                   
                    for (int z = 0; z < possibleCities.Count; z++)
                    {
                        //Check to see if tempCity is already in list
                        if (tempCity.Name.CompareTo(possibleCities[z].Name) == 0)
                        {

                            Console.WriteLine(tempCity.Name + " is already in the list.");
                            Console.ReadLine();
                            //If it is, update value if distance is less
                            if(tempCity.distToStart > (current.distToStart + (int) Distance(current, tempCity)))
                            {
                                tempCity.distToStart = current.distToStart + (int)Distance(current, tempCity);
                                tempCity.prevCity = current;
                                updated = true;
                                Console.WriteLine("Updating " + tempCity.Name);
                                Console.ReadLine();
                                TR_output.WriteLine("Updated:" + tempCity.Name + " " + tempCity.prevCity.Name);
                                break;
                            }
                        }
                    }

                    //If it wasnt in the list, then nothing was updated and we should add it to the list.
                    if (!updated)
                    {

                        Console.WriteLine(tempCity.Name + " was not in the list");
                        Console.ReadLine();
                        TR_output.WriteLine(tempCity.Name + " " + tempCity.x + " " + tempCity.y);

                        bool notinPossible = true;
                        foreach (var n in possibleCities)
                        {
                            if (tempCity.Name.CompareTo(n.Name) == 0)
                            {

                                Console.WriteLine("Updating the links for " + tempCity.Name);
                                Console.WriteLine(tempCity.Name + "'s previous city is now: " + tempCity.prevCity.Name);
                                
                                Console.ReadLine();
                                //tempCity.prevCity = current;
                                //tempCity.distToStart = current.distToStart + (int)Distance(current, tempCity); 
 
                                n.prevCity = tempCity.prevCity;
                                n.prevVisited = true;
                                n.distToStart = tempCity.distToStart;
                                notinPossible = false;
                                break;
                            }
                        }

                        
                        tempCity.prevCity = current;
                        tempCity.distToStart = current.distToStart + (int)Distance(current, tempCity);

                        if (notinPossible)
                        {
                            Console.WriteLine(tempCity.Name + " is not in list and is now being added.");
                            Console.WriteLine(tempCity.Name + "'s previous city is " + tempCity.prevCity.Name);
                            Console.ReadLine();
                            possibleCities.Add(tempCity);
                        }



                        //put back here
                        //tempCity.prevCity = current;
                        //tempCity.distToStart = current.distToStart + (int)Distance(current, tempCity);

                    }
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
         * Pre-Conditions:
         * Post-Condition: City Best
         * 
         * Determines the best possible city for the
         * A* algorithm to choose. Removes that city
         * from the list, and returns the best city
         * *************************************/
        public static City ComparePossibilities()
        {
            City Best = null;
            int currentMin = 1000000;

            foreach (var n in possibleCities)
            {
                if ((n.distToStart + CalculateEstimate(n)) < currentMin)
                {
                    currentMin = n.distToStart + CalculateEstimate(n);
                    Best = n;
                }
            }

            return Best;
       
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
            Console.WriteLine("Inside RemoveFromPossible! Possible cities count is " + possibleCities.Count);
            Console.ReadLine();
            for (int i = 0; i < possibleCities.Count; i++)
            {
                //Console.WriteLine(possibleCities[i].Name + " is in the list of possible");
                if (Best.Name.CompareTo(possibleCities[i].Name) == 0)
                {

                    foreach (var c in allCities)
                    {
                        if (possibleCities[i].Name.CompareTo(c.Name) == 0) c.prevVisited = true;
                    }

                    possibleCities.Remove(possibleCities[i]);

                    Console.WriteLine("Removing " + possibleCities.Count);
                    Console.ReadLine();



                    break;
                }
            }


            foreach (var c in possibleCities) Console.WriteLine(c.Name + " is in the list of possible");
            Console.ReadLine();
        }

        /***************************************
         * Function: UpdatePrevious
         * PreCondition: City Old
         * PostCondition: void
         * 
         * This takes in a city, and updates
         * the prevVisited and prevCity for the
         * corresponding city in allCities
         * *************************************/
        public static void UpdatePrevious(City toUpdate)
        {

            Console.WriteLine("Entered UpdatePrevious!  Passed " + toUpdate.Name);
            Console.ReadLine();

            for (int i = 0; i < allCities.Count; i++)
            {
                if (allCities[i].Name.CompareTo(toUpdate.Name) == 0)
                {
                    allCities[i].prevCity = toUpdate.prevCity;
                    allCities[i].prevVisited = toUpdate.prevVisited;

                    Console.WriteLine("Altered " + allCities[i].Name);
                    Console.ReadLine();
                    break;
                }
            }
        }

        /***************************************
        * Function: printPrevVisited
        * PreCondition: City
        * PostCondition: void
        * 
        * Prints out all the cities that were
        * visited by the algorithm
        * *************************************/
        public static void printPrevVisited(City Best)
        {



            TR_output.WriteLine("---------------------");
            TR_output.WriteLine("Visited Cities");
            
            for (int i = 0; i < allCities.Count; i++)
            {
                if (Best != null)
                {
                    if (allCities[i].Name.CompareTo(Best.Name) == 0)
                    {
                        allCities[i].prevVisited = true;
                    }

                    if (allCities[i].Name.CompareTo(Best.prevCity.Name) == 0)
                    {
                        allCities[i].prevVisited = true;
                    }
                }
                if (allCities[i].prevVisited)
                {
                    TR_output.Write(allCities[i].Name+ " ");
                }
            }
            TR_output.WriteLine();
        }

        public static void printList(City destination)
        {

            TR_output.WriteLine("------------------------");
            TR_output.WriteLine("Traversed Path:");

            if (destination == null || destination.Name.CompareTo("") == 0 )
            {
                TR_output.WriteLine("No Possible Path Found");
                return;
            }

            int place = 0;
            finalList.Add(destination.Name);

            Console.WriteLine(finalList[0]);
            Console.ReadLine();

            for (int i = 0; i < allCities.Count; i++)
            {
                if (allCities[i].Name.CompareTo(destination) == 0)
                {
                    place = i;
                    break;
                }
            }

            while (allCities[place].prevCity != null)
            {

                finalList.Add(allCities[place].prevCity.Name);

                Console.ReadLine();
                Console.WriteLine(allCities[place].prevCity.Name);

                for (int j = 0; j < allCities.Count; j++)
                {
                    if (allCities[j].Name.CompareTo(allCities[place].prevCity.Name) == 0)
                    {
                        place = j;
                        break;
                    }
                }
            }

            for (int k = finalList.Count - 1; k >= 0; k--)
            {
                TR_output.Write(finalList[k] + " ");
            }
            TR_output.WriteLine();
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
    }
}
