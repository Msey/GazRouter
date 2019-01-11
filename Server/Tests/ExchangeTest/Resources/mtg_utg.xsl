<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'
                xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>
  <xsl:param name='input' as='xs:string'   >
  </xsl:param>
  <xsl:param name='delimiter' >,</xsl:param>
  <xsl:param name='regex' >
    ^\s*([^:]*)\s*:\s*([^:]*)\s*:[^:]*$
  </xsl:param>
  <xsl:param name='lookupTableParam'>
    <value key='1'>
      <entityId>0278449B9A8A4FD1BC8A328542DE9902</entityId>
      <propertyTypeId>pressureInlet</propertyTypeId>
    </value>
  </xsl:param>
  <xsl:key name='lookup' match='value' use='@key'></xsl:key>
  <xsl:variable name='input-text' as='xs:string' select='unparsed-text($input)'/>
  <!--  <xsl:variable name='lines' as='xs:string*' select="tokenize($input, '\r?\n')"/>-->
  <xsl:variable name='lines' as='xs:string*' select='tokenize($input, "__")' />
  <xsl:variable name='parsed-lines' as='element(line)*'>
    <xsl:for-each select='$lines'>
      <xsl:analyze-string select='.' flags='x' regex='{$regex}'>
        <xsl:matching-substring>
          <xsl:message>
            matching line '<xsl:value-of select='.' />'
          </xsl:message>

          <xsl:variable name="value" as="xs:double" >
            <xsl:variable name="regex">
              \s*([-+]?\d*)<xsl:value-of select="$delimiter"></xsl:value-of>(\d+)\s*
            </xsl:variable>
            <xsl:analyze-string select='regex-group(2)' flags='x' regex="{$regex}" >
              <xsl:matching-substring>
                <xsl:value-of select="concat(regex-group(1),'.',regex-group(2))"/>
              </xsl:matching-substring>
              <xsl:non-matching-substring>
                <xsl:value-of select="."/>
              </xsl:non-matching-substring>
            </xsl:analyze-string>
          </xsl:variable>
          <line ID='{regex-group(1)}' value='{$value}' />
        </xsl:matching-substring>
        <xsl:non-matching-substring>
          <xsl:message>
            Non-matching line '<xsl:value-of select='.' />'
          </xsl:message>
        </xsl:non-matching-substring>
      </xsl:analyze-string>
    </xsl:for-each>
  </xsl:variable>
  <xsl:variable name="timestamp" as='xs:dateTime'>
    <xsl:analyze-string select="$lines[1]" flags="x" regex="[^:]*\s*:\s*(\d\d\d\d)(\d\d)(\d\d)(\d\d)(\d\d)(\d\d)\s*">
      <xsl:matching-substring>
        <xsl:value-of select="concat(regex-group(1),'-',regex-group(2),'-',regex-group(3), 'T', regex-group(4), ':', regex-group(5), ':', regex-group(6))"/>
      </xsl:matching-substring>
      <xsl:non-matching-substring>
        <xsl:value-of select="."/>
      </xsl:non-matching-substring>
    </xsl:analyze-string>
  </xsl:variable>

  <xsl:template match='/'>
    <ExchangeMessage xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
      <HeaderSection>
        <TimeStamp>
          <xsl:value-of select="$timestamp"></xsl:value-of>
        </TimeStamp>
        <PeriodType>Twohours</PeriodType>
        <GeneratedTime>2014-07-14T10:53:37</GeneratedTime>
      </HeaderSection>
      <DataSection>
        <ExchangeItems>
          <xsl:for-each select='$parsed-lines'>
            <xsl:variable name='rec' select='.'></xsl:variable>

            <ExchangeItem>
              <xsl:for-each select='key("lookup", @ID, $lookupTableParam)'>
                <Properties>
                  <Property>
                    <Value xsi:type="xsd:double">
                      <xsl:value-of select='$rec/@value'/>
                    </Value>
                    <PropertyTypeSysName>
                      <xsl:value-of select='propertyTypeId' />
                    </PropertyTypeSysName>
                  </Property>
                </Properties>
                <Formatted_ID>
                  <xsl:value-of select='entityId'></xsl:value-of>
                </Formatted_ID>
              </xsl:for-each>
            </ExchangeItem>
          </xsl:for-each>
        </ExchangeItems>
      </DataSection>
    </ExchangeMessage>
  </xsl:template>

</xsl:stylesheet>