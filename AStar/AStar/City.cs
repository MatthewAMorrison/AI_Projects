/**********************************************
 * Names: Matthew Morrison U56201087
 *      : Jonathan Palmer  U33353822
 * 
 * File Name: City.cs
 * Date: 21 FEB 2011
 * 
 * This file contains the class file for City
 * data
 * ********************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    class City
    {
        //Attributes
        public string Name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        List<City> connections = new List<City>();
        public bool prevVisited { get; set; }
        public City prevCity { get; set; }
        public int distToStart { get; set; }


        //Methods

        public bool isConnected(City city)
        {
            foreach (var c in this.connections)
            {
                if (c.Name == city.Name) { return true; }
                else { return false; }
            }

            return false;
        }

        public bool isVisited()
        {
            return this.prevVisited;
        }

        public void addConnection(City theCity)
        {
            connections.Add(theCity);
        }

        public int getConnectionTotal()
        {
            return connections.Count;
        }

        public string getConnectionName(int i)
        {
            return connections[i].Name;
        }

        public City getConnection(int i)
        {
            return connections[i];
        }
    }
}
