﻿Author: Tanner Wilson.
Initial commit date: 9/28/2015
Implementation:
	This SpreadSheet solution is the framework for a larger GUI driven Spreadsheep application. 
	This framework utelizes two lower level classes: Formula and Dependency Graph. Both of these classes
	are used to represent portions of cells in the spreadsheet. The Formula class is used to hold and evaluate
	inputs that could be stored in cells, while the Graph class is a way to keep track of when each cell's
	contents should be evaluated. 
	(e.g. if cell B1 depends on A1 and cell C1 depends on B1, the Spreadsheet should evaluate in the order
	A1 -> B1 -> C1 and so on)
	The Graph class will also support in finding which cell should be evaluated first.
	This Spreadsheet solution brings all these classes together to begin a working Spreadsheet application.
	
	README update: 9/28/2015  
