<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >
    14.08.2015 1
    121,60.5,,,,
    1503 1,0X0 2,0 3,50.4  4,50.4  5,50.4  6,50.4  7,8  8,8  9,8   10,17.8   11,4   12,0  13,0  14 ,
    1506,0,50.9,8,,,,,
    1507,0,,51,51,0,0,0,,0,,,,,
    1509,,,,,,52.3,,,,,,,,
    1513,0,,40.3,40.3,40.3,40.3,0,,0,,,,,
    161,60.5,,,,
    1801,0X0,0,60.3,60.3,66.5,66.5,6,12,12,14.3,0,0,0,
    1803,0X0,0,66.6,66.6,66.6,66.6,15.7,15.7,15.7,13,7,1,0,
    1805,0X0,0,62.1,62.1,62.1,62.1,15,15,15,14.2,7,1,0,
    1807,2X1,6.1667,55.6,55.1,75,74.2,11.2,17.9,17.9,10.3,3,0,15,
    1809,0X0,0,72.1,72.1,72.1,72.1,10,10,10,15.3,5,3,0,
    1811,0X0,,69.7,69.7,69.7,69.7,10.2,10.2,10.2,19.6,8,0,0,
    1813,0X0,0,65.2,65.2,65.2,65.2,10,12,12,15.9,7,1,0,
    1815,0X0,,60.3,60.3,60.3,60.3,10,10,10,14.9,8,0,0,
    1817,2X1,6.8333,55.3,54.8,64.8,64,8,23,23,16.8,4,2,0,
    1819,0X0,0,59.2,59.2,59.2,59.2,10,10,10,19,6,0,0,
    1823,0X0,,62,62,62,62,9,9,9,19.2,4,4,0,
    1825,0X0,0,55.3,55.3,55.3,55.3,9,9,9,18.1,7,0,0,
    1827,2X1,8.1667,49.5,49,68.2,67.4,8,30.8,30.8,21.6,4,1,27,
    1833,0X0,0,57.5,57.5,71.2,71.2,7,7.2,7.2,14.6,6,2,0,
    1841,0X0,0,60.3,60.3,66.5,66.5,6,12,12,14.3,8,0,0,
    1843,0X0,0,66.6,66.6,66.6,66.6,15.7,15.7,15.7,13,0,8,0,
    1845,0X0,0,65.6,65.6,65.6,65.6,15,15,15,14.2,7,1,0,
    1847,0X0,0,56.6,56.6,74.8,74.8,10,22,22,10.3,7,1,0,
    1849,0X0,0,71.7,71.7,71.7,71.7,10,10,10,15.3,7,1,0,
    1851,0X0,,69.7,69.7,69.7,69.7,10.2,10.2,10.2,19.6,8,0,0,
    1853,0X0,0,65.2,65.2,65.2,65.2,10,11.1,11.1,15.9,6,2,0,
    1855,0X0,,60.3,60.3,60.3,60.3,10,10,10,14.9,8,0,0,
    1857,1X1,3.1667,55.4,54.9,64.9,64.1,8,23,23,16.8,4,2,0,
    1859,0X0,0,59.2,59.2,59.2,59.2,10,10,10,19,7,1,0,
    1863,0X0,,62,62,62,62,9,9,9,19.2,7,1,0,
    1865,0X0,0,55.4,55.4,55.4,55.4,9,9,9,18.1,7,1,0,
    1867,1X2,,49.2,48.7,68.2,67.4,8,28.6,28.6,21.6,6,0,20,
    1871,2265,67.1,28,,,,,
    1872,2260,67.1,29,,,,,
    1887,2X1,8.8333,56.2,55.7,75.2,74.4,12.3,25,25,10.3,3,1,20,
    1901,0X0,8.875,52.9,52.4,67.3,66.5,10,23.6,23.6,16.2,4,1,26,
    1903,1X1,4.4167,53.2,52.7,67.3,66.5,10,20.5,20.5,16.2,4,0,23,
    1913,0X0,0,58.4,58.4,71.1,71.1,12,11.7,11.7,14.6,4,1,0,
    1933,4X1,16.9583,47.5,47,64.5,63.7,5,20.4,20.4,14.3,2,0,28,
    1941,2X1,8.875,52.9,52.4,67.3,66.5,10,23.6,23.6,16.2,4,1,26,
    1973,,,,,
    1975,0X0,0,47.4,47.4,63.5,63.5,5,19.4,19.4,14.3,6,0,69,
    2013,0X0,0,64.9,64.9,64.9,64.9,5,5,5,14.3,4,2,0,
    2015,3X1,13.4167,47.5,47,64.6,63.8,5,19.5,19.5,14.3,4,0,87,
    21,1X1,,60.5,60,73.1,72.3,19,,34,16,,,,
    23,0X0,,66.5,66.5,66.5,66.5,18,,66.5,16,,,,
    25,2X1,,60.4,59.9,70.1,69.3,16,,28,18,,,,
    3354,0,,,,,,,
    3998,7.3217,,,,,,,
    3999,7.3217,60,,,,,,
    4000,0,,,,,,,
    4001,.2832,,,,,,,
    4002,.0006,,,,,,,
    4003,.0088,,,,,,,
    4004,.0049,,,,,,,
    4005,.0242,,,,,,,
    4006,.354,,,,,,,
    7033,.455,,,,,,,
    4558,3.8,,,,,,,
    4559,0,,,,,,,
    4578,.3367,,,,,,,
    5314,.2754,,,,,,,
    5505,319,60.5,19.7,,,,,
    5507,0,,,,,,,
    5508,0,,,,,,,
    5510,197.9,69.1,20.2,,,,,
    61,2X2,,60.5,60,73,72.2,19,,37,16,,,,
    6284,0,0,0,,,,,
    63,0X0,,66.5,66.5,66.5,66.5,18,,18,16,,,,
    65,0X0,,61.2,61.2,53,53,16,,28,18,,,,
    6842,2704.8,58.8,0,,,,,
    6844,3698.6,46.6,-1,,,,,
    6845,0,56.8,-1,,,,,
    6846,3041.4,58.8,-1,,,,,
    6847,3236.6,46.2,-1,,,,,
    6851,1650,68.3,28.3,,
    6947,1270,67.8,21.2,,
    6948,1400.4,69.9,23.4,,
    6949,2.8461,60,,,,,,
    6953,1.7828,,,,,,,
    6954,8.6,,,,,,,
    6955,.9781,,,,,,,
    6956,0,0,5.5,0,,,,
    6957,.2754,,,,,,,
    6958,0,,,,,,,
    6959,.0901,,,,,,,
    6960,.4911,,,,,,,
    6961,.4475,,,,,,,
    6962,.0283,,,,,,,
    6963,0,,,,,,,
    6964,.1555,,,,,,,
    6965,.3658,,,,,,,
    6966,.6769,,,,,,,
    6967,0,,,,,,,
    6968,,,,,,,,
    6969,.2383,,,,,,,
    6970,.5275,,,,,,,
    6971,.9399,,,,,,,
    6972,.5887,,,,,,,
    0,,,,,,,,
    6973,1.2873,,,,,,,
    6975,.0244,,,,,,,
    6976,.4164,,,,,,,
    6977,.1606,,,,,,,
    6978,,,,,,,,
    6999,1253.9167,,,,,,,
    7003,0,56.2,0,10,,
    7006,4522.979,67.1,28,,,,,
    7007,59.807,,,,,,,
    7012,0,0,,,,,,
    7017,965,67.2,16.6,,
    7018,980,67.6,30,,
    7020,388.3,51.6,23.5,,,,,,
    7021,388.3,0,,,,,,
    7024,193.9,,,,,,,
    7025,115,,,,,,,
    7026,210,,,,,,,
    7031,3.4424,,,,,,,
    7032,1.9419,,,,,,,
    7035,.878,,,,,,,
    7036,273,,,,,,,
    7037,3.011,,,,,,,
    7040,6936,48.5,5,,
    7041,0,,72.2,14.2,,,,
    7051,0,,,,,,,
    7052,0,,72.2,18.1,,,,
    7058,300,43,,10,0,,,
    7059,0,0,,0,0,,,
    7061,699,27.5,,10,12.9,,,,
    7062,6.3529,,,,,,,
    7063,0,,,,,,,
    7067,61,66.6,4.5,,
    7096,244.1,66.6,7.8,,
    7702,14743.5833,,,,,,,
    7704,1211.7,60.3,,,,,,
    7705,1319.5,60.3,66.5,,,,,
    7706,,,66.5,,,60.3,,,,,,,,,
    7707,,,66.5,,,60.3,,,,,,,,,
    7708,94,60.9,12,,,,
    7887,.1438,,,,,,,
    8017,2796,60.9,14,,
    8335,1136,49.7,5,,
    8336,265.9,69.9,23.2,,
    6979,.3573,,,,,,,
    9220,66,27.8,,7,13.4,,,
    9221,0,27.8,7,16.1,,,,
    9222,207,27.7,10.1,13.2,,,,
    9608,.0696,,,,,,,
    9696,.1812,,,,,,,
    4897,.0454,,,,,,,
    4848,26.9874,,,,,,,
    6913,199,,66,,33.5,,
    9832,0X0,0,45.9,45.9,45.9,45.9,-3,5,5,11.8,7,0,0,
    9833,0X0,0,45.9,45.9,45.9,45.9,-3,5,5,11.8,7,0,0,
    9834,3X1,12.9167,45.7,45.2,70.1,69.3,-3,19,19,11.8,3,1,139,
    9835,0X0,12.9167,45.7,45.2,70.1,69.3,-3,19,19,11.8,3,1,139,
    9830,3X1,11.5833,45.9,45.4,61.5,60.7,-3,18.6,18.6,11.8,4,0,133,
    8853,190,48.7,7,,
    3695,866,51,13,,
  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>

  <!-- метка времени сеанса-->
  <xsl:param name='regexTimestamp' >(\d{2}).(\d{2}).(\d{4})</xsl:param>
  <xsl:param name="timestamp">
    <xsl:analyze-string select="$rows[2]" flags="x" regex='{$regexTimestamp}'>
      <xsl:matching-substring>
        <xsl:value-of select="concat(regex-group(1),'-',regex-group(2),'-',regex-group(3) ,'T00:00:00')"/>
      </xsl:matching-substring>
    </xsl:analyze-string>
  </xsl:param>

  <!-- тип периода сеанса -->
  <xsl:param name='periodType'>Twohours</xsl:param>

  <!-- разделитель строк входного файла-->
  <xsl:param name='columnDelimiter'>,</xsl:param>

  <xsl:template match='/'>
    <ExchangeMessage>
      <HeaderSection>
        <TimeStamp>
          <xsl:value-of select="$timestamp"/>
          <xsl:message>
            <xsl:value-of select='$timestamp' />
          </xsl:message>
        </TimeStamp>
        <PeriodType>
          <xsl:value-of select="$periodType"/>
        </PeriodType>
      </HeaderSection>
      <DataSection>
        <ExtItems>
          <xsl:for-each select='$rows'>
            <!-- разбивка строки на колонки -->
            <xsl:variable name='currentRow'  select='.'/>
            <xsl:variable name='splitRow' as='xs:string*' select="tokenize($currentRow, $columnDelimiter)"/>
            <!-- данные по КЦ -->
            <xsl:if test="count($splitRow) = 15">
              <xsl:message>
                KC<xsl:value-of select='$splitRow' />
              </xsl:message>
              
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','compressorShopPattern') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[2]"/>
                </Value>
              </ExtItem>
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','fuelGasConsumption') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[3]"/>
                </Value>
              </ExtItem>
              
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','pressureInlet') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[5]"/>
                </Value>
              </ExtItem>
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','pressureOutlet') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[6]"/>
                </Value>
              </ExtItem>
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','temperatureInlet') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[8]"/>
                </Value>
              </ExtItem>
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','temperatureOutlet') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[9]"/>
                </Value>
              </ExtItem>
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','temperatureCooling') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[10]"/>
                </Value>
              </ExtItem>
              
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','compressorUnitsInReserve') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[12]"/>
                </Value>
              </ExtItem>
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','compressorUnitsUnderRepair') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[13]"/>
                </Value>
              </ExtItem>
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','coolingUnitsInUse') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[14]"/>
                </Value>
              </ExtItem>
            </xsl:if>
            
            <!--данные по ЗЛ-->
            <xsl:if test="count($splitRow) = 6">
              <xsl:message>
                GIS<xsl:value-of select='$splitRow' />
              </xsl:message>
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','flow') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[2]"/>
                </Value>
              </ExtItem>
                            
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','pressureInlet') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[3]"/>
                </Value>
              </ExtItem>

              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','temperatureInlet') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[4]"/>
                </Value>
              </ExtItem>
              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','densityAbsolute') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[5]"/>
                </Value>
              </ExtItem>

              <ExtItem>
                <ExtKey>
                  <xsl:value-of select="normalize-space( concat($splitRow[1],'_','combustionHeatLow') )"/>
                </ExtKey>
                <Value>
                  <xsl:value-of select="$splitRow[6]"/>
                </Value>
              </ExtItem>
              
            </xsl:if>
            <!--<xsl:message>1#<xsl:value-of select='$currentRow' /></xsl:message>
                    <xsl:message>2#<xsl:value-of select='count($splitRow)' /></xsl:message>-->
          </xsl:for-each>
        </ExtItems>
      </DataSection>
    </ExchangeMessage>
  </xsl:template>
</xsl:stylesheet>