<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'
                xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8' />
  <xsl:param name='input' as='xs:string'></xsl:param>

  <xsl:param name='regex'>
    ^\s*([^;]*)\s*;\s*([^;]*)\s*;\s*([^;]*)\s*$
  </xsl:param>
  <xsl:param name='lookupTableParam' >
    <!--<LookupTable >
      <Items>
        <Item >
          <ExtKey>4003;2</ExtKey>
          <EntityId>0278449B9A8A4FD1BC8A328542DE9902</EntityId>
          <PropertyTypeId>PressureInlet</PropertyTypeId>
        </Item>
      </Items>
    </LookupTable>-->
  </xsl:param>

  <xsl:key name='lookup' match='Item' use='ExtKey'></xsl:key>
  <xsl:variable name='input-text' as='xs:string' select='unparsed-text($input)' />
  <!--  <xsl:variable name='lines' as='xs:string*' select="tokenize($input, '\r?\n')"/>-->
  <xsl:variable name='lines' as='xs:string*' select='tokenize($input, "__")' />

  <xsl:variable name='parsed-lines' as='element(line)*'>
    <xsl:for-each select='$lines'>
      <xsl:analyze-string select='.' flags='x' regex='{$regex}'>
        <xsl:matching-substring>
          <xsl:message>
            matching line '<xsl:value-of select='.' />'
          </xsl:message>

          <xsl:variable name="value">
            <xsl:value-of select="regex-group(3)" />
          </xsl:variable>
          <xsl:variable name="id" as="xs:string">
            <xsl:value-of select="concat(regex-group(1), ';', regex-group(2))" />
          </xsl:variable>
          <line ID='{$id}' value='{$value}' />
        </xsl:matching-substring>
        <xsl:non-matching-substring>
          <xsl:message>
            Non-matching line '<xsl:value-of select='.' />'
          </xsl:message>
        </xsl:non-matching-substring>
      </xsl:analyze-string>
    </xsl:for-each>
  </xsl:variable>


  <xsl:template match='/'>
    <ExchangeMessage xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
      <HeaderSection>
        <PeriodType>Twohours</PeriodType>
      </HeaderSection>
      <DataSection>
        <ExchangeItems>
          <xsl:for-each select='$parsed-lines'>
            <xsl:variable name='rec' select='.'></xsl:variable>
            <xsl:variable name="type">
              <xsl:choose>
                <xsl:when test="@ID = '4001;1100' or @ID = '4003;1100' or @ID = '4005;1100'">
                  <xsl:value-of>xsd:string</xsl:value-of>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of>xsd:double</xsl:value-of>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:variable>

            <ExchangeItem>
              <xsl:for-each select='key("lookup", @ID, $lookupTableParam)'>
                <Properties>
                  <Property>
                    <Value xsi:type='{$type}'>
                      <xsl:value-of select='$rec/@value' />
                    </Value>
                    <PropertyTypeSysName>
                      <xsl:value-of select='PropertyTypeId' />
                    </PropertyTypeSysName>
                  </Property>
                </Properties>

                <Item>
                  <Id>
                    <xsl:value-of select='EntityId'></xsl:value-of>
                  </Id>
                </Item>  
            
              </xsl:for-each>
            </ExchangeItem>
          </xsl:for-each>
        </ExchangeItems>
      </DataSection>

    </ExchangeMessage>
  </xsl:template>

</xsl:stylesheet>