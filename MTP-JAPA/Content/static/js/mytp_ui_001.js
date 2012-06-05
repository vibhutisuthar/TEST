		$(document).ready(function() 
			{
			function initialise_smartphone() 
				{
				//hide the following
				$('#ultabs').hide();
				
				$('#menu').click(function(e)
					{
					if( $(this).hasClass("maximised") ) {
						$('#ultabs').slideUp(900);
						$(this).removeClass("maximised").addClass("minimised");
						}
					else {
						$('#ultabs').slideDown(900);
						$(this).removeClass("minimised").addClass("maximised");
						}
					return false;	
					});

			function initialise_desktop() 
				{
				//select the children of booking ref and hide them
				$('#booking_information').addClass("maximised");
				$('article.segment').addClass("maximised");
				}
			
				if( $(window).width() < 600 )
				{
					initialise_smartphone();
				}
				else
				{
					initialise_desktop();
				}
			
			});