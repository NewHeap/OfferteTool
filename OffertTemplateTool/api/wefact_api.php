<?php

class WeFactAPI
{
	
	private $url;
	private $responseType;
	private $apiKey;
	
	function __construct(){
		$this->url			 = 'localhost/wefact/Pro/apiv2/api.php';
		$this->api_key		 = '280dbf8499b1622ed007e06fd2068dc9';
	}
	
	public function sendRequest($controller, $action, $params){
		
		if(is_array($params)){
			$params['api_key'] 		= $this->api_key; 
			$params['controller'] 	= $controller;
			$params['action'] 		= $action;
		}
		
		$ch = curl_init();
		curl_setopt($ch,CURLOPT_URL, $this->url);
		curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, 0);
		curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, FALSE);
		curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
		curl_setopt($ch, CURLOPT_TIMEOUT,'10');
		curl_setopt($ch, CURLOPT_POST, 1);
		curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($params));
		$curlResp = curl_exec($ch);
		$curlError = curl_error($ch);
		
		if ($curlError != ''){
			$result = array(
				'controller' => 'invalid',
				'action' => 'invalid',
				'status' => 'error',
				'date' => date('c'),
				'errors' => array($curlError)
			);
		}else{
			$result = json_decode($curlResp, true);
		}
		
		return $result;
	}
}

function print_r_pre($data){
	echo "<pre>";
	print_r($data);
	echo "</pre>";
}
?>