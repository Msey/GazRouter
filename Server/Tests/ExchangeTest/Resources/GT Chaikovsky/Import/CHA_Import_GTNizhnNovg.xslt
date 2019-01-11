<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >Volgotransgaz
    1; 30052017; 12; 69.26; 69.3; 21; 0;
    2; 30052017; 12; 69.07; 69.1; 22; 0;
    3; 30052017; 12; 63.8; 63.8; 22; 0;
    4; 30052017; 12; 63.32; 63.41; 22; 0;
    5; 30052017; 12; 54.01; 66.81; 17.2; 3;
    6; 30052017; 12; 54.62; 66.26; 11.99; 1;
    7; 30052017; 12; 55.34; 73.18; 12.17; 1;
    8; 30052017; 12; 55; 73.35; 11.24; 2;
    9; 30052017; 12; 56.04; 73.04; 7; 0;
    10; 30052017; 12; 54.76; 73.63; 11.97; 3;
    11; 30052017; 12; 55.11; 73.52; 12.16; 3;
    12; 30052017; 12; 55.99; 72.87; 7; 0;
    13; 30052017; 12; 10592.3; ; ; ;
    14; 30052017; 12; 59.96; 60.02; 10; 0;
    15; 30052017; 12; 58.4; 58.4; 10; 0;
    16; 30052017; 12; 57.76; 57.77; 10; 0;
    17; 30052017; 12; 57.81; 57.79; 10; 0;
    18; 30052017; 12; 53.85; 73.09; 16.11; 2;
    19; 30052017; 12; 53.85; 73.09; 16.11; 2;
    20; 30052017; 12; 1869.3; ; ; ;
    21; 30052017; 12; 36.51; 33.5; 13; 0;
    22; 30052017; 12; 34.59; 34.2; 13; 0;
    23; 30052017; 12; 29.65; 44.61; 12.69; 2;
    24; 30052017; 12; 678; ; ; ;
    25; 30052017; 12; -967; ; ; ;
    26; 30052017; 12; 56.7; 72.4; 10; 3;
    27; 30052017; 12; 56.2; 72.4; 12; 6;
    28; 30052017; 12; 56.2; 72.4; 12; 2;
    29; 30052017; 12; 56.3; 72.1; 16; 3;
    30; 30052017; 12; 56.3; 72.1; 16; 3;
    31; 30052017; 12; 61.2; 61.2; 17; 1;
    32; 30052017; 12; 36.1; 36.1; 11; 5;
    33; 30052017; 12; 36.1; 36.1; 11; 5;
    END
  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>

  <!-- метка времени сеанса-->
  <xsl:param name='regexTimestamp' >(\d{1,4});(\d{2})(\d{2})(\d{4});(\d{2});</xsl:param>
  
  <xsl:param name="timestamp">
        <xsl:analyze-string select="translate($rows[2] , ' ','')" flags="x" regex='{$regexTimestamp}'>
            <xsl:matching-substring><xsl:value-of select="concat(regex-group(2),'-',regex-group(3),'-',regex-group(4) ,'T', regex-group(5), ':00:00')"/>
            </xsl:matching-substring>
      </xsl:analyze-string>
  </xsl:param>

  <!-- тип периода сеанса -->
  <xsl:param name='periodType'>Twohours</xsl:param>

  <!-- разделитель строк входного файла-->
  <xsl:param name='columnDelimiter'>;</xsl:param>
  
  <xsl:template match='/'>
    <ExchangeMessage>
      <HeaderSection>
        <TimeStamp>
          <xsl:value-of select="$timestamp"/>
          <xsl:message><xsl:value-of select='$timestamp' /></xsl:message>
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
            
             <!--<xsl:message>mess:<xsl:value-of select='$currentRow' /></xsl:message>-->
            
                <xsl:variable name='splitRow' as='xs:string*' select="tokenize($currentRow, $columnDelimiter)"/>
              <!-- данные по КЦ -->
                <xsl:if test="count($splitRow) = 8">
                      <!--<xsl:message>KC<xsl:value-of select='$splitRow' /></xsl:message>-->
                      <ExtItem>
                        <ExtKey><xsl:value-of select="normalize-space( concat($splitRow[1],'_','pressureInlet') )"/></ExtKey>
                        <Value><xsl:value-of select="$splitRow[4]"/></Value>
                      </ExtItem>
                      <ExtItem>
                        <ExtKey><xsl:value-of select="normalize-space( concat($splitRow[1],'_','pressureOutlet') )"/></ExtKey>
                        <Value><xsl:value-of select="$splitRow[5]"/></Value>
                      </ExtItem>
                      <ExtItem>
                        <ExtKey><xsl:value-of select="normalize-space( concat($splitRow[1],'_','temperatureInlet') )"/></ExtKey>
                        <Value><xsl:value-of select="$splitRow[6]"/></Value>
                      </ExtItem>
                      <ExtItem>
                        <ExtKey><xsl:value-of select="normalize-space( concat($splitRow[1],'_','compressorUnitsInUse') )"/></ExtKey>
                        <Value><xsl:value-of select="$splitRow[7]"/></Value>
                      </ExtItem>
                     <!--
                      <ExtItem>
                        <ExtKey><xsl:value-of select="normalize-space( concat($splitRow[1],'_','compressorUnitsInReserve') )"/></ExtKey>
                        <Value><xsl:value-of select="$splitRow[8]"/></Value>
                      </ExtItem>
                      <ExtItem>
                        <ExtKey><xsl:value-of select="normalize-space( concat($splitRow[1],'_','temperatureOutlet') )"/></ExtKey>
                        <Value><xsl:value-of select="$splitRow[9]"/></Value>
                      </ExtItem>
                      <ExtItem>
                        <ExtKey><xsl:value-of select="normalize-space( concat($splitRow[1],'_','temperatureCooling') )"/></ExtKey>
                        <Value><xsl:value-of select="$splitRow[10]"/></Value>
                      </ExtItem>
                      -->
                </xsl:if>
                <!--данные по ГИС-->
              <!--
                <xsl:if test="count($splitRow) = 5">
                      <xsl:message>GIS<xsl:value-of select='$splitRow' /></xsl:message>
                      <ExtItem>
                        <ExtKey><xsl:value-of select="normalize-space( concat($splitRow[1],'_','flow') )"/></ExtKey>
                        <Value><xsl:value-of select="$splitRow[4]"/></Value>
                      </ExtItem>
                </xsl:if>
                -->
                <!--данные по КС-->
              <!--
                <xsl:if test="count($splitRow) = 6">
                  <xsl:message>KS<xsl:value-of select='$splitRow' /></xsl:message>
                  <ExtItem>
                    <ExtKey><xsl:value-of select="normalize-space( concat($splitRow[1],'_','pressureAir') )"/></ExtKey>
                    <Value><xsl:value-of select="$splitRow[4]"/></Value>
                  </ExtItem>
                  <ExtItem>
                    <ExtKey><xsl:value-of select="normalize-space( concat($splitRow[1],'_','temperatureAir') )"/></ExtKey>
                    <Value><xsl:value-of select="$splitRow[5]"/></Value>
                  </ExtItem>
                </xsl:if>
                -->
                <!--<xsl:message>1#<xsl:value-of select='$currentRow' /></xsl:message>
                    <xsl:message>2#<xsl:value-of select='count($splitRow)' /></xsl:message>-->
          </xsl:for-each>
        </ExtItems>
      </DataSection>
    </ExchangeMessage>
  </xsl:template>
</xsl:stylesheet>