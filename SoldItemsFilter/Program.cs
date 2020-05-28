namespace SoldItemsFilter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class SoldItem : IComparable
    {
        private int id;
        private String name;
        private String serialNumber;
        private int cost;
        private int salesPrice;
        private String salesPerson;
        private int? salesPersonProfit;

        /*
         * Constructor for the SoldItem Class. salesPersonProfit gets initialized by static SetProfits(List<SoldItem> soldItems) method
         */
        public SoldItem(int id, String name, String serialNumber, int cost,
            int salesPrice, String salesPerson)
        {
            this.id = id;
            this.name = name;
            this.serialNumber = serialNumber;
            this.cost = cost;
            this.salesPrice = salesPrice;
            this.salesPerson = salesPerson;
        }

        /*
         * CompareTo Method for IComparable interface so that a list of SoldItems can be sorted (descending)
         * in terms of profit
         */
        public int CompareTo(object obj)
        {
            if (obj is SoldItem)
            {
                if ((this.salesPrice - this.cost) - ((obj as SoldItem).salesPrice - (obj as SoldItem).cost) > 0)
                {
                    return -1;
                }
                else if ((this.salesPrice - this.cost) - ((obj as SoldItem).salesPrice - (obj as SoldItem).cost) < 0)
                {
                    return 1;
                }
                else if (this.serialNumber.CompareTo((obj as SoldItem).serialNumber) > 0) // if the profitablity of items are the same, sort by serialnumber (descending)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            throw new ArgumentException("Object is not a SoldItem");
        }

        /*
         * toString method to print a readable version of the SoldItem Class
         */
        public String toString()
        {
            return "id: " + this.id + ", name: " + this.name + ", serialNumber: " +
                this.serialNumber + ", cost: " + this.cost + ", salesPrice: " +
                this.salesPrice + ", salesPerson: " + this.salesPerson + ", SalesPerson's Profit: " + this.salesPersonProfit;
        }

        /*
         * Main Function for SoldItem Class First
         * Filters out items from list based off of 
         *  - if they are not profitable (if price < cost, there is no profit)
         *  - if the item was sold more than once
         *  - serial number consists of the capital letters of their name joined with their ID(punctuation and casing ignored)
         *  After being filtered, the list is sorted by the most profitable
         *  Time complexity: O(n^2), due to complexity of SetProfts() method
         *  Space complexity: O(n), due to space complexity of FindDuplicates
         *  @param items: The list of sold items to filter and sort
         *  @return a new list of sold items, filtered and sorted according to criteria
         */
        public static List<SoldItem> SoldItemsFilter(List<SoldItem> items)
        {
            // static method that stores the profits of each salesPerson with each item
            SetProfits(items); //O(n ^ 2) time, this method should be an area to target for refactoring

            // remove all items not sold at a profit
            items.RemoveAll(notProfitable);

            //get duplicate Serial Codes
            List<string> dupSerials = FindDuplicates(items); //O(n) time

            List<SoldItem> itemSoldOnce = new List<SoldItem>(); // create new list for items only sold once

            // add all items that were only sold once to a new list
            foreach (SoldItem i in items) //O(n)
            {
                //if serial code was not duplicated, item was sold only 1 time
                if (!dupSerials.Contains(i.serialNumber))
                {
                    itemSoldOnce.Add(i);
                }
            }

            //filter out items without proper serial code
            List<SoldItem> filteredItems = SerialFormat(itemSoldOnce); //O(n) time

            //sort the list from most profitable items to least profitable items
            filteredItems.Sort();
            // This Sort() method used quick sort, which is on average O(nlog(n)), but in the worst case O(n^2)
            // For refactoring, could implement a merge sort method to make worst case O(nlog(n))
            /*http://msdn.microsoft.com/en-us/library/b0zbh7b6.aspx */

            //group items by the profitability of its Sales Person
            var query = filteredItems.GroupBy(item => item.salesPersonProfit); //O(n), since list has already been sorted

            List<SoldItem> finalItems = new List<SoldItem>(); //create a final list to add filtered and sorted items to

            foreach (var sItems in query)
            {
                foreach (var soldItem in sItems)
                {
                    finalItems.Add(soldItem);
                }
            }

            return finalItems;
        }

        /*
         * Static boolean Predicate to determine if an item is not profitable
         * ex salesprice <= cost
         */
        private static bool notProfitable(SoldItem item)
        {
            if (item.salesPrice <= item.cost)
            {
                return true;
            }
            return false;
        }

        /*
         * Helper function to find items sold more than once
         * Returns list of serial numbers for items that occur
         * multiple times in the inputed list
         * Time complexity: O(n) where where n is length of items list
         * Space complexity: O(n) where where n is length of items list (since all items are either added to the original list or duplicate list)
         * @param items: The list of sold items to search through
         * @return: A list of strings for the serial numbers that occur more than once in the items list
         */
        public static List<string> FindDuplicates(List<SoldItem> items)
        {
            List<string> origSnumbers = new List<string>();
            List<string> duplicateSnumbers = new List<string>();

            foreach (SoldItem item in items)
            {
                if (origSnumbers.Contains(item.serialNumber)) //if our original list contains the serial number, we have a duplicate
                {
                    duplicateSnumbers.Add(item.serialNumber);
                }
                else
                {
                    origSnumbers.Add(item.serialNumber);
                }
            }

            return duplicateSnumbers;
        }

        /*
         * Iterates through a list of SoldItems, and returns a new list with only items in the format:
         * serial number consists of the capital letters of item name joined with item ID (punctuation and casing ignored)
         * Time complexity: O(n) where where n is length of items list
         * Space complextity: O(m) where m is number of items in list m that contain properly formatted serial numbers
         * @param items: The list of sold items to search through
         * @return: a list with only items with properly formated serial numbers
         */
        public static List<SoldItem> SerialFormat(List<SoldItem> items)
        {
            List<SoldItem> serialFormattedItems = new List<SoldItem>();

            foreach (SoldItem item in items)
            {
                string itemName = @item.name;
                //build a string that contains all the capital letters of the name variable in SoldItem
                string capitalLetters = string.Concat(itemName.Where(c => c >= 'A' && c <= 'Z'));

                //remove the punctuation from the serial number for equality comparison and turn all letter to uppercase, since we want to ingore case for comparison
                string serialNoPunctuation = item.serialNumber.Replace("-", "").ToUpper(); //ASSUME punctuation is only a '-' character, could adjust if we were expecting different
                //types of punction

                //build a regex that is just all the capital letters of a name plus the id of the item
                Regex rx = new Regex(capitalLetters + item.id);


                //if the regex matches, add the item to the returned list, check both cases since we are ignoring case on the serial code
                if (rx.IsMatch(serialNoPunctuation))
                {
                    serialFormattedItems.Add(item);
                }

            }
            return serialFormattedItems;

        }

        /*
         * Static method that returns the profit of a particular sales Person (given by name)
         * and a list of sold items
         * Time complexity: O(n) where n is length of soldItems list 
         * Space complexity: O(1), size does not depend on input
         * @param salesPersonName: The name of the salesPerson
         * @param items: The list of sold items to search through
         * @return Total Profit from all sales of an individual sales person
         */
        public static int CalculateSalesPersonProfit(string salesPersonName, List<SoldItem> items)
        {
            int profit = 0;

            foreach (SoldItem item in items)
            {
                if (salesPersonName.Equals(item.salesPerson))
                    profit = profit + (item.salesPrice - item.cost);
            }

            return profit;

        }

        /*
         * Static method that sets salesPersonProfit attribute of SoldItem. Area to target for refactoring efficiency.
         * Time complexity: O(n^2) where n is length of items list 
         * Space complexity: O(m) where m is amount of unique names in items list
         * @param soldItems: The list of sold items to search through
         */
        public static void SetProfits(List<SoldItem> soldItems)
        {
            List<string> salesPeople = new List<string>();

            foreach (SoldItem item in soldItems) //add the names of all the salespeople to its own list O(n)
            {
                if (!salesPeople.Contains(item.salesPerson)) //add to list if name is not already in it
                {
                    salesPeople.Add(item.salesPerson);
                }
            }

            foreach (string person in salesPeople) //set the profits field for each sales person
            // O(n^2) where n is length of items list and 
            {
                int profit = CalculateSalesPersonProfit(person, soldItems); //O(n)

                //set person's profits everywhere
                foreach (SoldItem item in soldItems)
                {
                    if (item.salesPerson.Equals(person))
                    {
                        item.salesPersonProfit = profit;
                    }
                }
            }
        }

        // Main Method to print output to console
        public static void Main()
        {
            //initialize data
            List<SoldItem> soldItems = new List<SoldItem>
            {
                  new SoldItem(1,"Rock Salt", "RS1",10,50,"Andy Ghadban"),
                  new SoldItem(2,"Planter's Nuts", "XO28-V",4,23,"Reginald VelJohnson"),
                  new SoldItem(3,"Bulk Pack SuperWash Fire Hoses", "BPSW-FH3",122,122,"Harry Lewis"),
                  new SoldItem(4,"BlackBOX carnival sticks", "BBOX4",215,460,"Jean-Luc Picard"),
                  new SoldItem(5,"ARMY surplus Canned Beef", "5-ARMYCB",34,513,"Jean-Luc Picard"),
                  new SoldItem(6,"Compressed Air", "CA6",80,900,"Frank Castle"),
                  new SoldItem(7,"Rock Salt", "RS1",10,2,"Reginald VelJohnson"),
                  new SoldItem(8,"Werther's Original", "WO-8",12,75,"Andy Ghadban"),
                  new SoldItem(9,"tonka truck passenger door", "TT-PD-9",336,275,"Jean-Luc Picard"),
                  new SoldItem(10,"ARMY surplus Canned Beef", "5-ARMYCB",12,6000,"Frank Castle"),
                  new SoldItem(11,"SwashBuckler's Buckled Swashes", "SBBS11",122,160,"Harry Lewis"),
                  // extra soldItems for testing
                  //new SoldItem(12,"ROck Salt", "ROS12",10,11,"Andy Ghadban"),
                  //new SoldItem(13,"PlAnter's Nuts", "XO28-V",4,23,"Reginald VelJohnson"),
                  //new SoldItem(14,"Bulk Pack SuperWash Fire Hoses", "BPSW-FH3",122,122,"Harry Lewis"),
                  //new SoldItem(15,"BlackBOX carnival sticks", "BBOX4",215,460,"Jean-Luc Picard"),
                  //new SoldItem(16,"ARMY surplus Canned Beef", "5-ARMYCB",34,513,"Jean-Luc Picard"),
                  //new SoldItem(17,"CompresseD Air", "cdA17",80,90,"Frank Castle"),
                  //new SoldItem(18,"Rock Salt", "RS1",10,2,"Reginald VelJohnson"),
                  //new SoldItem(19,"WErther's Original", "WEO-19",12,75,"Andy Ghadban"),
                  //new SoldItem(20,"tonka truck passenger door", "TT-PD-20",336,275,"Jean-Luc Picard"),
                  //new SoldItem(21,"ARMY surplus Canned Beef", "5-ARMYCB",12,6000,"Frank Castle"),
                  //new SoldItem(22,"SwashBuckler's Buckled Swashes", "SBBS11",122,160,"Harry Lewis")

            };

            List<SoldItem> original = soldItems;
            SetProfits(original); //for the sake of output I am calling SetProfits in main() as well
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Original List: ");
            foreach (SoldItem item in original)
            {
                // Display Original List
                Console.WriteLine(item.toString());
            }
            List<SoldItem> filteredAndSorted = SoldItemsFilter(soldItems);


            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Filtered and Sorted List: ");

            foreach (SoldItem item in filteredAndSorted)
            {
                // Display Filtered and Sorted List
                Console.WriteLine(item.toString());
            }

        }
    }
}

