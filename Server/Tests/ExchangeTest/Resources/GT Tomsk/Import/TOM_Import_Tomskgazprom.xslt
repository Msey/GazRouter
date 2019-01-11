<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'
                xmlns:rs='urn:schemas-microsoft-com:rowset' 
                xmlns:z='#RowsetSchema'
                xmlns:s='uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882' 
                xmlns:dt='uuid:C2F41010-65B3-11d1-A29F-00AA00C14882'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- метка времени сеанса-->
  <xsl:param name="timestamp">
    <xsl:variable name='dtStr' select="xml/rs:data/z:row/@DateTime"/>
    <xsl:variable name='dtHour' select="xml/rs:data/z:row/@Hour"/>
    <xsl:variable name='day' select="substring($dtStr,1,10)" />
    <xsl:value-of select="concat($day,'T',$dtHour,':00:00')" />
  </xsl:param>
  <!-- тип периода сеанса -->
  <xsl:param name='periodType'>Twohours</xsl:param>
  
  <xsl:template match='xml/s:Schema/s:ElementType'>
    <ExchangeMessage>
      <HeaderSection>
        <TimeStamp>
          <xsl:value-of select="$timestamp"/>
        </TimeStamp>
        <PeriodType>
          <xsl:value-of select="$periodType"/>
        </PeriodType>
      </HeaderSection>
      <DataSection>
        <ExtItems>
          <xsl:for-each select='s:AttributeType'>
              <xsl:variable name='param' select ='@name'/>
              <ExtItem>
                  <ExtKey>
                      <xsl:value-of select ='$param'/>
                  </ExtKey>
                <Value>
                  <xsl:for-each select='../../../rs:data/z:row[position()=1]/@*'>
                    <xsl:if test='name() = $param'>
                        <xsl:value-of select='.'/>
                    </xsl:if>
                  </xsl:for-each>
                </Value>
              </ExtItem>
          </xsl:for-each>
          
        </ExtItems>
      </DataSection>
    </ExchangeMessage>
  </xsl:template>

  
</xsl:stylesheet>