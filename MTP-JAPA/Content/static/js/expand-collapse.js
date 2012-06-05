		$(document).ready(function() 
			{
			function initialise_smartphone() 
				{
				//select the children of booking ref and hide them
				$('#passenger_contact_details, #agent_details, #datestamp').hide();
				$('#ultabs').hide();
				$('#booking_information').addClass("minimised");
				$('article.segment').addClass("minimised");
				$('section#compliance > div').addClass("minimised");
				$('.seg_details, .seg_notes, .seg_contact','article.segment').hide();
				}
			
			function initialise_desktop() 
				{
				//select the children of booking ref and hide them
				$('#booking_information').addClass("maximised");
				$('article.segment').addClass("maximised");
				}
			
			function setup_events()
				{
				$('#booking_ref').click(function(e)
					{
					if( $(this).hasClass("maximised") ) {
						$('section[id!="booking_reference"]','#passenger_contact_details, #agent_details').slideUp(900);
						$(this).removeClass("maximised").addClass("minimised");
						}
					else {
						$('section[id!="booking_reference"]','.itin_passenger').slideDown(900);
						$(this).removeClass("minimised").addClass("maximised");
						}
					return false;	
					});
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
				$('section#compliance > div > h1').click(function(e)
					{
					var $seg = $(this).parents("div");
					if( $seg.hasClass("minimised") ) {
						//select the parent article element for scope
						$seg.removeClass("minimised").addClass("maximised");
						}
					else {
						$seg.removeClass("maximised").addClass("minimised");
						}
					return false;
					});

				$('.seg_main','.segment').click(function(e)
					{
					var $seg = $(this).parents("article.segment");
					if( $seg.hasClass("minimised") ) {
						//select the parent article element for scope
						$seg.removeClass("minimised").addClass("maximised");
						$seg.find('.seg_details, .seg_notes, .seg_contact').slideDown(900);
						}
					else {
						$seg.removeClass("maximised").addClass("minimised");
						$seg.find('.seg_details, .seg_notes, .seg_contact').slideUp(900);
						}
					return false;
					});

				$('#booking_information').click(function(e)
					{
					var $seg = $(this);
					if( $seg.hasClass("minimised") ) {
						$seg.removeClass("minimised").addClass("maximised");
						$seg.find('#passenger_contact_details, #agent_details').slideDown(900);
						$('#datestamp').slideDown(900);
						}
					else {
						$seg.removeClass("maximised").addClass("minimised");
						$seg.find('#passenger_contact_details, #agent_details').slideUp(900);
						$('#datestamp').slideUp(900);
						}
					return false;
					});
				} //setup_events
			
			if( $(window).width() < 980 )
				{
				initialise_smartphone();
				}
			else
				{
				initialise_desktop();
				}
			
			setup_events();
			
			});