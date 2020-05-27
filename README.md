# SoldItemsFilterWithNet
Paul Macfarlane  
  
Test this program by running static main() method in SoldItem.cs  
  
A c# program with a function that takes in a list of sold items and filters the sold items in such a way that includes only:  
* only profitable items (SalesPrice - Cost >0)  
* that were sold exactly once (Only one instance of the product name and serial number)  
* serial number consists of the capital letters of their name joined with their ID (punctuation and casing ignored)  
  
After the list is filtered, the resulting list in descending order of the overall most profitable salespeople (sales people with greatest difference between SalesPrices and Costs).  
  
Programming Decisions made/implemented: (SoldItem.cs also contains detailed comments explaining though process):  
* Created a SoldItem data structure to store the data for each sold items  
* Stored all soldItems in a generic list of SoldItem  
* Created the primary function SoldItemsFilter(List soldItems) to filter and sort all items according to criteria above  
* Within SoldItemsFilter, call SetProfits(), which calculates the profit for every sales person and updates the field within SoldItem to stores the total profit of a sales person. This attribute is used to sort soldItems list later on  
* Remove all items that are not profitable (salesPrice - cost)  
* Find all items in soldItems that were sold only once, and then add those to a new list  
* From this new list, filter out all items with invalid serialNumbers (see criteria at top of readme)  
* Sort the list of filtered items based off of profitability of the items sales person (descending)  
* SoldItem implements the IComparable Interface and the corresponding CompareTo() function, so the list of sold items can be sorted using the generic List sorting method (quicksort)  
* Group the remain solditems by the profitability of each salesPerson, so that the remaining list is now completed filtered and sorted by profitablity of each salesPerson. Within each group, the items are sorted by total profit (descending)  
* Return the filtered and sorted list  
  
Assumptions Made:  
* The data for soldItems can be stored in a SoldItem Object  
* The punction for serial codes of a SoldItem is limited to a "-" character.  
* When sorting by most profitable salesperson, the sorting criteria is based off of the original data, not the filtered data  
  
Time/Space Complexity of SoldItemsFilter(soldItems):  
* Worst Case Time Efficiency is O(n^2 * m), where n is number of items in the list of sold Items, and m is the amount of unique items.
* Worst Cast Space Efficiency is O(n), where n is number items in soldItems. A few methods within SoldItemsFilter() create a new generic list, which in the worst case will be size n.  
* This worst case time efficiency is due to the method SetProfits() called inside of SoldItemsFilter(), where SetProfits is called once with a nested loop structure. The SetProfits static method is an area to target for refactoring efficiency.  
* Specific details for the time and space complexity of each method can be found in the comments and headers of functions in SoldItem.cs  
  
Copy of this repository without .net project files is located at: https://github.com/PaulMacSiena/SoldItemsFilter
