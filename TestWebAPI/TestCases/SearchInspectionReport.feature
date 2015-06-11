Feature: SearchInspectionReport
	Target: api/DynamicPage/GetViewResult/{entityName}
	Test Scenarios: 
		1. Search All Inspection Reports;
		2. Search Inspection Report by primary project(lookup);
		3. Search Inspection Report by status(status);
		4. Search Inspection Report by report date(datetime);

Background: 
	Given Exist following Inspection Reports
	| # | PImpProjAutoID | PContractAutoID   | ContractPerson | InspectorID | WorkPerformed | ReportStatusID | ReportDate |
	| 1 | 1111 - 1111    | cofee02 - cofee02 | 12 - 12        | 1           | WorkPerformed | Approved       | 2015-1-3   |
	| 2 | 107520 - tt    | cofee02 - cofee02 | 12 - 12        | 1           | WorkPerformed | Draft          | 2015-1-3   |
	| 3 | 107520 - tt    | cofee02 - cofee02 | 12 - 12        | 1           | WorkPerformed | Approved       | 2015-1-3   |
	| 4 | 107520 - tt    | cofee02 - cofee02 | 12 - 12        | 1           | WorkPerformed | Approved       | 2015-1-4   |

@p3
Scenario: Search All Inspection Reports
	When Send a request to api:GetViewResult/InspectionReport
	| RequestParameter | Value |
	| _QueryType       | 0     |
	| _Page            | 0     |
	| _PageSize        | 0     |
	Then These Inspection Reports should be searched out:#1,#2,#3,#4

@p3
Scenario: Search Inspection Report By Conditions
	When Send a request to api:GetViewResult/InspectionReport
	| RequestParameter  | Value                 |
	| _QueryType        | 1                     |
	| _Page             | 0                     |
	| _PageSize         | 0                     |
	| A0_PImpProjAutoID | 107520 - tt           |
	#| A0_ReportStatusID     | Approved |--unsearchable attribute
	| A0_ReportDate     | 2015-01-03,2015-01-03 |
	Then These Inspection Reports should be searched out:#2,#3
	And These Inspection Reports should not be searched out:#1,#4
