<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet 
                xmlns:xhtml="http://www.w3.org/1999/xhtml"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:fo="http://www.w3.org/1999/XSL/Format"
                version="1.0" exclude-result-prefixes="xsl fo xhtml">



  <xsl:output encoding="UTF-8" method="xml" indent="yes" omit-xml-declaration="yes"/>
  <xsl:output method="html" />
  
  <xsl:template match="raw">
	<p style="font-weight: bold; color: #ffcc00;"><xsl:value-of select="@form"/></p>
  </xsl:template>

  <xsl:template match="tbxml">
  </xsl:template>
	  
  <xsl:template match="tbheader">
  </xsl:template>
  
  <xsl:template match="tbdoc">
	<div id="print_wrapper">
        <h1 class="outline">Travel Bytes Complete Itinerary for American Express Business Travel</h1>
        <header>
          <h2 class="outline">Header</h2>
          <xsl:value-of select="Header/HTMLDATA/." disable-output-escaping="yes" />
        </header>

        <footer>
          <h2 class="outline">Footer</h2>
          <xsl:value-of select="Footer/HTMLDATA/." disable-output-escaping="yes" />
        </footer>
    
        <xsl:apply-templates select="BookingInformation"/>

        <xsl:apply-templates select="PriorityRemarks"/>

        <div id="ad_wrapper">

            <section id="ad_column2">
              <img class="ad1" src="/content/static/graphics/ad_1.jpg" width="160" height="160" alt="Ad 1" title="Ad 1" />
              <img class="ad2" src="/content/static/graphics/ad_2.jpg" width="160" height="160" alt="Ad 2" title="Ad 2" />
              <img class="ad3" src="/content/static/graphics/ad_3.jpg" width="160" height="600" alt="Ad 3" title="Ad 3" />
            </section> <!-- / #ad_column2 -->

          <section id="segments">
		
            <h1 class="outline">Travel Details</h1>
		  
            <xsl:apply-templates select="TravelDetails"/>

          </section> <!-- / #segments -->

          <xsl:apply-templates select="AirlineRecord"/>

          <xsl:apply-templates select="Compliance"/>
		  
            <xsl:apply-templates select="AdColumn"/>

            <section id="ad_column">
              <h1 class="outline">Travel Advertisements</h1>
              <img class="ad1" src="/content/static/graphics/ad_1.jpg" width="160" height="160" alt="Ad 1" title="Ad 1" />
              <img class="ad2" src="/content/static/graphics/ad_2.jpg" width="160" height="160" alt="Ad 2" title="Ad 2" />
              <img class="ad3" src="/content/static/graphics/ad_3.jpg" width="160" height="600" alt="Ad 3" title="Ad 3" />
            </section> <!-- / #ad_column -->

	    </div><!-- / #ad_wrapper -->
	</div><!-- / #print_wrapper -->
  </xsl:template>


  <xsl:template match="BookingInformation">


    <section id="booking_information" class="ads">
      <h1 class="outline">Booking Information</h1>

      <section id="booking_reference">
        <h1>Booking Reference</h1>
        <p class="reference">
          <xsl:value-of select="@Ref"/>
        </p>
      </section>
      <!-- / #booking_reference -->


      <section id="passenger_details">
        <h1 class="outline">Passenger Details</h1>

        <section id="travellers">
          <h1 class="outline">
            <a href="#travellers">Travellers</a>
          </h1>
          <div class="label"><span>Travel Arrangements for</span>
            <xsl:apply-templates select="PassengerDetails"/>
		  </div>
        </section>
        <!-- / #travellers -->

        <section id="booking_reference2">
          <span>Booking Reference</span>
          <p class="reference">
            <xsl:value-of select="@Ref"/>
          </p>
        </section>
        <!-- / #booking_reference2 -->

      
        <section id="passenger_contact_details">
          <h1>
            <a href="#passenger_contact_details">Passenger Contact Details</a>
          </h1>
          <xsl:apply-templates select="PassengerContactDetails"/>
        </section>
        <!-- / #passenger_contact_details -->
      
      </section>
      <!-- / #passenger_details -->

      <section id="agent_details">
        <h1>
          <a href="#agent_details">Agent Details</a>
        </h1>
        <xsl:apply-templates select="AgentDetails"/>
      </section>
      <!-- / #agent_details -->

    </section>
    <!-- / #booking_information -->

    
  </xsl:template>
  
  <xsl:template match="PassengerDetails">

    
    <h2 class="passenger_name">
      <span class="name_first">
        <xsl:value-of select="@FirstName"/>
      </span>
      <xsl:text> </xsl:text>
      <span class="name_surname">
        <xsl:value-of select="@SurName"/>
      </span>
    </h2>
    
  </xsl:template>

  <xsl:template match="AgentDetails">
    <p><xsl:value-of select="@AgentTitle"/></p>
    <p>
      <xsl:value-of select="@Address1"/>
    </p>
    <p>
      <xsl:value-of select="@Address2"/>
    </p>
    <p>
      <xsl:value-of select="@Address3"/>
    </p>
    <div class="label">
      <span>Telephone Intl</span>
      <p>
	    <xsl:value-of select="@PhoneINTL"/>
	  </p>
    </div>
    <div class="label">
      <span>Local</span>
      <p>
	    <xsl:value-of select="@AgentLocal"/>
	  </p>
    </div>
    <div class="label">
      <span>Reg No</span>
      <p>
	    <xsl:value-of select="@AgentRegNo"/>
	  </p>
    </div>
  </xsl:template>

  <xsl:template match="PassengerContactDetails">
    <div class="label">
      <span>Contact</span>
      <p>
	    <xsl:value-of select="@Contact"/>
	  </p>
    </div>
    <div class="label">
      <span>Phone</span>
      <p>
	    <xsl:value-of select="@Phone"/>
      </p>
    </div>
    <div class="label">
      <span>Email</span>
      <p>
	    <xsl:value-of select="@Email"/>
	  </p>
    </div>
  </xsl:template>


  <xsl:template match="PriorityRemarks">
    <section id="priority_remarks">
      <h1 class="outline">
        <xsl:apply-templates/>
        I AM PRIORITY REMARK
      </h1>
    </section>
    <!-- / priority_remarks -->
  </xsl:template>


  <xsl:template match="TravelDetails">
    <xsl:apply-templates select="Date"/>
  </xsl:template>

  <xsl:template match="Date">


    
    <section class="date">
      <h1>
        <span>Travel Details</span>
        <xsl:value-of select="@Date"/>
      </h1>

      <xsl:for-each select="Date/.">
        <xsl:sort select="SegmentNumber" case-order="upper-first"/>
      </xsl:for-each>

      <xsl:for-each select="child::*">
        <xsl:sort select="@SegmentNumber" order="ascending" data-type="number"/>

        <!--<xsl:value-of select="@SegmentNumber"/>
        <xsl:value-of select="name(.)" />-->
        
        <xsl:apply-templates select="."/>

      </xsl:for-each>


      <!--
        <xsl:apply-templates select="../TourInformation"/>
        <xsl:apply-templates select="../FlightInformation"/>
        <xsl:apply-templates select="../HotelInformation"/>
        <xsl:apply-templates select="../CarInformation"/>
        <xsl:apply-templates select="../RailInformation"/>
        <xsl:apply-templates select="../MiscInformation"/>
        <xsl:apply-templates select="../OpenInformation"/>
        -->


    </section>
    <!-- / .date -->
    
  </xsl:template>





  <xsl:template match="AdColumn">

	<section id="ad_column">
		<h1>Travel Advertisements</h1>
		<img src="/content/static/graphics/ad_1.jpg" width="160" height="160" alt="Ad 1" title="Ad 1" />
		<img src="/content/static/graphics/ad_2.jpg" width="160" height="160" alt="Ad 2" title="Ad 2" />
		<img src="/content/static/graphics/ad_3.jpg" width="160" height="600" alt="Ad 3" title="Ad 3" />
	</section>
	<!-- / #ad_column -->

  </xsl:template>



  <xsl:template match="FlightInformation">

    <article class="segment flight">
      <h1>Flight Information</h1>

      <div class="seg_main">
        <header>
          <p class="flight_number"><xsl:value-of select="@FlightNo"/></p>
          <h2 class="label">
            <span>Airline</span>
            <span class="data">
              <xsl:value-of select="@Airline"/>
            </span>
          </h2>
          <h3 class="label">
            <span>Flight</span>
            <span class="data">
              <xsl:value-of select="@FlightNo"/>
            </span>
          </h3>
          <p class="outline">
            <xsl:value-of select="@Date"/>
          </p>
        </header>

        <h4 class="outline">Departure / Arrival</h4>
        <div class="label dep_city">
          <span class="mobile">Departure</span>
          <span class="computer">Origin</span>
          <p><xsl:value-of select="@DepartureCity"/></p>
        </div>
        <div class="label arr_city">
          <span>Destination</span>
          <p><xsl:value-of select="@ArrivalCity"/></p>
        </div>
        <div class="label dep_time">
          <span>Departing</span>
          <p><xsl:value-of select="@DepartureTime"/></p>
        </div>
        <div class="label arr_time">
          <span>Arriving</span>
          <p><xsl:value-of select="@ArrivalTime"/></p>
        </div>
        <div class="label dep_terminal">
          <span>Departure Terminal</span>
          <p><xsl:value-of select="@DepartureTerminal"/></p>
        </div>
        <div class="label arr_terminal">
          <span>Arrival Terminal</span>
          <p><xsl:value-of select="@ArrivalTerminal"/></p>
        </div>
        <div class="label dep_date">
          <span>Departure Date</span>
          <p><xsl:value-of select="@DepartureDate"/></p>
        </div>
        <div class="label dep_terminal2">
          <span>Departure Terminal</span>
          <p>Terminal <xsl:value-of select="@DepartureTerminal"/></p>
        </div>
        <div class="label arr_city2">
          <span>Arrival</span>
          <p><xsl:value-of select="@ArrivalCity"/></p>
        </div>
        <div class="label arr_time2">
          <span>Arriving</span>
          <p><xsl:value-of select="@ArrivalTime"/></p>
        </div>
        <div class="label arr_date">
          <span class="blank">Arrival Date</span>
          <p><xsl:value-of select="@ArrivalDate"/></p>
        </div>
        <div class="label arr_terminal2">
          <span>Arrival Terminal</span>
          <p>Terminal <xsl:value-of select="@ArrivalTerminal"/></p>
        </div>
        <div class="label status2">
          <span>Status</span>
          <p><xsl:value-of select="@Status"/></p>
        </div>
      </div>
      <!-- / .seg_main -->

      <div class="seg_details">
        <h4>Details</h4>
        <div class="label status">
          <span>Status</span>
          <p><xsl:value-of select="@Status"/></p>
        </div>
        <div class="label">
          <span class="mobile">Est. Time</span>
          <span class="computer">Estimated Time</span>
          <p><xsl:value-of select="@EstimatedTime"/></p>
        </div>
        <div class="label">
          <span>Meal Service</span>
          <p><xsl:value-of select="@SupplierService"/></p>
        </div>
        <div class="label">
          <span class="mobile">Stops</span>
          <span class="computer">Number of Stops</span>
          <p><xsl:value-of select="@SupplierStops"/></p>
        </div>
        <div class="label">
          <span>Seat</span>
          <p><xsl:value-of select="@Seat"/></p>
        </div>
      </div>
      <!-- / .seg_details -->

      <div class="seg_notes">
        <h4 class="outline">Segment Notes</h4>
        <div class="label">
          <span>Class</span>
          <p><xsl:value-of select="@Class"/></p>
        </div>
        <div class="label">
          <span>Operated by</span>
          <p><xsl:value-of select="@Operator"/></p>
        </div>
      </div>
      <!-- / .seg_notes -->
    </article> <!-- / .flight -->
    
  </xsl:template>

  <xsl:template match="TourInformation">
    <article class="segment hotel">
      <h1>Tour Information</h1>
      <div class="seg_main">
        <header>
        </header>
        <div class="checkin">
          <h4 class="outline">Tour Start Date</h4>
          <div class="label">
            <span>Status</span>
            <p><xsl:value-of select="@Status"/></p>
          </div>
          <div class="label">
            <span>Tour Start Date</span>
            <p><xsl:value-of select="@StartDate"/></p>
          </div>
          <p>
            <xsl:value-of select="@Description"/>
          </p>
        </div>
      </div>    
    </article> <!-- / .tour -->
  </xsl:template>

  

  <xsl:template match="HotelInformation">

    <article class="segment hotel">
      <h1>Hotel Information</h1>

      <div class="seg_main">
        <header>
          <h2 class="label">
            <span>Hotel</span>
            <span class="data">
              <xsl:value-of select="@HotelName"/>
            </span>
          </h2>
          <h3 class="outline">
            <span>Reference Number</span>
            <span class="data">
              <xsl:value-of select="@Ref"/>
            </span>
          </h3>
          <p class="outline"><xsl:value-of select="@Date"/></p>
        </header>

		<div class="secondary">
		  <div class="checkin">
            <h4 class="outline">Check In</h4>
            <div class="label">
              <span class="mobile">Check In</span>
              <span class="computer">Check In Date</span>
              <p><xsl:value-of select="@Checkin"/></p>
            </div>
		  </div>

          <div class="checkout">
            <h4 class="outline">Check Out</h4>
            <div class="label">
              <span class="mobile">Check Out</span>
              <span class="computer">Check Out Date</span>
              <p><xsl:value-of select="@Checkout"/></p>
            </div>
          </div>

          <div class="label ref">
            <span class="mobile">Reference</span>
            <span class="computer">Reference Number</span>
            <p><xsl:value-of select="@Ref"/></p>
          </div>

        </div>
      </div>
      <!-- / .seg_main -->

      <div class="seg_details">
        <h4>Details</h4>
        <div class="label status">
          <span>Status</span>
          <p><xsl:value-of select="@Status"/></p>
        </div>
        <div class="label">
          <span>Stay</span>
          <p><xsl:value-of select="@Stay"/></p>
        </div>
        <div class="label">
          <span>Guaranteed By</span>
          <p><xsl:value-of select="@Guaranteed"/></p>
        </div>
      </div>

      <div class="seg_notes">
        <h4 class="outline">Segment Notes</h4>
        <div class="label">
          <span class="mobile">Est. Rate</span>
          <span class="computer">Estimated Rate</span>
          <p><xsl:value-of select="@EstimatedRate"/></p>
        </div>
      </div>

      <div class="seg_contact">
        <h4 class="outline">Hotel Details</h4>
        <div class="label">
          <span>Address</span>
          <p><xsl:value-of select="@Address"/></p>
        </div>
        <div class="label">
          <span>Telephone</span>
          <p><xsl:value-of select="@Telephone"/></p>
        </div>
      </div>
    </article>
    <!-- / .hotel -->

  </xsl:template>

  <xsl:template match="CarInformation">
    
    <article class="segment car">
      <h1>Car Information</h1>

      <div class="seg_main">
        <header>
          <h2 class="label">
            <span>Car Hire Company</span>
            <span class="data">
              <xsl:value-of select="@RentalCompany"/>
            </span>
          </h2>
          <h3 class="outline">
            <span>Confirmation No.</span>
            <span class="data">
              <xsl:value-of select="@Ref"/>
            </span>
          </h3>
          <p class="outline"><xsl:value-of select="@Date"/></p>
        </header>

        <div class="pickup">
          <h4 class="outline">Pick Up</h4>
          <div class="label">
            <span>Pick Up</span>
            <p><xsl:value-of select="@Pickup"/></p>
          </div>
          <div class="label address">
            <span class="mobile">Address</span>
            <span class="computer">Pick Up Address</span>
            <p><xsl:value-of select="@PickupAddress"/></p>
          </div>
        </div>

        <div class="dropoff">
          <h4 class="outline">Drop Off</h4>
          <div class="label">
            <span>Drop Off</span>
            <p><xsl:value-of select="@DropOff"/></p>
          </div>
          <div class="label address">
            <span class="mobile">Address</span>
            <span class="computer">Drop Off Address</span>
            <p><xsl:value-of select="@DropOffAddress"/></p>
          </div>
        </div>

        <div class="label ref">
          <span class="mobile">Confirmation</span>
          <span class="computer">Confirmation No.</span>
          <p><xsl:value-of select="@Ref"/></p>
        </div>
      </div>
      <!-- / .seg_main -->

      <div class="seg_details">
        <h4>Details</h4>
        <div class="label">
          <span>Size</span>
          <p><xsl:value-of select="@Size"/></p>
        </div>
        <div class="label">
          <span>???</span>
          <p>???AUTOMATIC???</p>
        </div>
        <div class="label">
          <span>???</span>
          <p>???A/C???</p>
        </div>
      </div>

      <div class="seg_notes">
        <h4 class="outline">Segment Notes</h4>
        <div class="label">
          <span>Rate</span>
          <p><xsl:value-of select="@Rate"/></p>
        </div>
        <div class="label">
          <span>Rate Code</span>
          <p><xsl:value-of select="@RateCode"/></p>
        </div>
        <div class="label">
          <span>Rate Type</span>
          <p><xsl:value-of select="@RateType"/></p>
        </div>
        <div class="label">
          <span>Mileage</span>
          <p><xsl:value-of select="@Mileage"/></p>
        </div>
      </div>
    </article>
    <!-- / .car -->
    
  </xsl:template>

  <xsl:template match="RailInformation">
    <article class="segment rail">
      <h1>Rail Information</h1>

      <div class="seg_main">
        <header>
          <h2 class="label">
            <span>Rail Supplier</span>
            <span class="data">
              <xsl:value-of select="@RailSupplier"/>
            </span>
          </h2>
          <h3 class="label">
            <span>Reference Number</span>
            <span class="data">
              <xsl:value-of select="@Ref"/>
            </span>
          </h3>
          <p class="outline"><xsl:value-of select="@Date"/></p>
        </header>
      </div>
      <!-- / .seg_main -->
    </article>
    <!-- / .rail -->
  </xsl:template>

  <xsl:template match="MiscInformation">
    <article class="segment misc">
      <h1>Misc Information</h1>

      <div class="seg_main">
        <header>
          <h2 class="label">
            <span>Misc. Supplier</span>
            <span class="data">
              <xsl:value-of select="@MiscSupplier"/>
            </span>
          </h2>
          <h3 class="label">
            <span>Reference Number</span>
            <span class="data">
              <xsl:value-of select="@Ref"/>
            </span>
          </h3>
          <p class="outline"><xsl:value-of select="@Date"/></p>
        </header>
      </div>
      <!-- / .seg_main -->
    </article>
    <!-- / .misc -->
  </xsl:template>

  <xsl:template match="OpenInformation">
    <article class="segment open">
      <h1>Open Information</h1>

      <div class="seg_main">
        <header>
          <h2 class="label">
            <span>Open Supplier</span>
            <span class="data">
              <xsl:value-of select="@OpenSupplier"/>
            </span>
          </h2>
          <h3 class="label">
            <span>Reference Number</span>
            <span class="data">
              <xsl:value-of select="@Ref"/>
            </span>
          </h3>
          <p class="outline"><xsl:value-of select="@Date"/></p>
        </header>
      </div>
      <!-- / .seg_main -->
    </article>
    <!-- / .open -->
  </xsl:template>


  <xsl:template match="AirlineRecord">
    <section id="airline_record">
      <h1>Airline Record Locators</h1>
      <div class="label">
        <span>Airline Ref</span>
        <p><xsl:value-of select="@Ref"/></p>
      </div>
    </section>
  </xsl:template>
  

  <xsl:template match="Compliance">
    <section id="compliance">
      <xsl:value-of select="HTMLDATA/." disable-output-escaping="yes"/>
    </section>
    <!-- / #compliance -->
  </xsl:template>

</xsl:stylesheet>
