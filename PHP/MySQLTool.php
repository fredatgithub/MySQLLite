<?PHP
//echo "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>";
class Obj 
{
	public $records;
	public $columns;
	public $data;
}
	
function ListTables($json)
{
	//{ "debug":1, "mode":0, "dbname":"vivobaby"}
	//Data Base	
	include ("config.php");
	
	$obj = json_decode($json);	
	$debug = $obj->debug;
    $dbname = $obj->dbname;

	//連結MYSQL	
	$link = mysql_connect($HostSqlIP, $HostSqlUser, $HostSqlPass)
	        or die("Error to link DB");

	//選擇資料庫
 	mysql_select_db($dbname)
	  		or die ("Can't select DB ".mysql_error()); 
 		
	//設定為UTF-8
    mysql_query("SET CHARACTER SET 'utf8'");
 

    if ($debug)
	{
		//echo '<pre>';
		//print_r($obj);
		//echo '</pre>';
	}
     
	$jsonArray = array();
	
	//設定SQL查詢字串
	$sql = "SHOW TABLES FROM ".$dbname;
	$result = mysql_query($sql)
	  				or die( "Search Error! ".$sql );
	if ($result) 
	{		
    	while ($row = mysql_fetch_row($result)) 
		{    		
			$tableName = $row[0];
			array_push($jsonArray, $tableName);							
		}//end while
		
	}//end if
	mysql_free_result($result);
	//關閉連結
	mysql_close($link); 
	
	echo json_encode($jsonArray);
}

function QueryDB($json)
{
	//{ "debug":1, "mode":"1", "sql":"SELECT * FROM devagingtestreport", "dbname":"vivobaby", "table":"devagingtestreport"}
	//Data Base	
	include ("config.php");
	
	$obj = json_decode($json);	
	$debug = $obj->debug;
    $sql = $obj->sql;
    $dbname = $obj->dbname;
    $table = $obj->table;
    
	
	
	//連結MYSQL	
	$link = mysql_connect($HostSqlIP, $HostSqlUser, $HostSqlPass)
	        or die("Error to link DB");

	//選擇資料庫
 	mysql_select_db($dbname)
	  		or die ("Can't select DB ".mysql_error()); 
 		
	//設定為UTF-8
    mysql_query("SET CHARACTER SET 'utf8'");
 

    if ($debug)
	{
		//echo '<pre>';
		//print_r($obj);
		//echo '</pre>';
	}
	
	$jsonArray = array();
	$colsName = array();
	
	//---get columns-----------------------------------
	//$result = mysql_query("SHOW COLUMNS FROM ".$table);
	//if ($result) 
	//{	
	//	while ($row = mysql_fetch_array($result)) 
	//	{
    //   		array_push($colsName, $row['Field']);			   
	//	}				
	//}
	//mysql_free_result($result);
	
	//---sql-------------------------------------------
	$result = mysql_query($sql)
	  				or die( "Search Error! ".$sql );
	  
	//---get columns-----------------------------------			
	while ($i < mysql_num_fields($result))
	{
   		$fld = mysql_fetch_field($result, $i);
   		array_push($colsName, $fld->name);	
   		$i = $i + 1;
	}

	$total_records = mysql_num_rows($result);
	
		
	$obj = new Obj();
	$obj->records = $total_records;
	$obj->columns = $colsName;
	$obj->data = array();


	for ($i=0; $i<$total_records; $i++)
	{	
		$itemArray = array();
		foreach ($colsName as $name)
		{
			$data = mysql_result($result, $i, $name);
			array_push($itemArray, $data);
		}
		array_push($obj->data, $itemArray);
	}
	
	mysql_free_result($result);
	//關閉連結
	mysql_close($link); 
	
	echo json_encode($obj);
}

function Insert($json)
{
	//{ "debug":1, "mode":"2", "sql":"SELECT * FROM devagingtestreport", "dbname":"vivobaby", "table":"devagingtestreport"}
	//Data Base	
	include ("config.php");
	
	$obj = json_decode($json);	
	$debug = $obj->debug;
    $sql = $obj->sql;
    $dbname = $obj->dbname;
    $table = $obj->table;
    
	if (strlen($sql) == 0 || strlen($dbname) == 0 || strlen($table) == 0) 
	{
		echo "Error:have param is null.";
		return false;	
	}		
	
	//連結MYSQL	
	$link = mysql_connect($HostSqlIP, $HostSqlUser, $HostSqlPass)
	        or die("Error to link DB");
	
	//選擇資料庫
      mysql_select_db($dbname)
	  		or die ("Can't select DB ".mysql_error()); 
	  		
	//設定為UTF-8
      mysql_query("SET CHARACTER SET 'utf8'");

	//執行SQL字串 
	$result = mysql_query($sql)
	    	      or die( "Error:".mysql_error()." SQL:".$sql);
	
	//關閉連結
	mysql_close($link); 
	
	if ($result) 
		echo "Success";
}

function MyDisptch($json)
{
	$obj = json_decode($json);
	
	switch ($obj->mode)
	{
	case 0:
		ListTables($json);
		break;
	case 1:
		QueryDB($json);
		break;
	case 2:
		Insert($json);
		break;
	} 
}
$json = file_get_contents('php://input'); 
MyDisptch($json)
?>