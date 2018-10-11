$('document').ready(function(){

	var flagForSettings = false;

	$('.fa-cog').on('click', function(){
		if(flagForSettings == false){

			$('#wrapperForSettings').css('display', 'block');	
			flagForSettings = true;
		}else{

			$('#wrapperForSettings').css('display', 'none');	
			flagForSettings = false;
		}
	});
	$('#getButton').on('click', function(){

		if(flagForSettings == true){

			$('#wrapperForSettings').css('display', 'none');	
			flagForSettings = false;	
		}
	});


});