<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
  xmlns:func="http://MyFunction"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text"  encoding="CP866" />

  <xsl:decimal-format NaN="" />

  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:variable name="timestamp">
    <xsl:value-of select="format-dateTime(ExchangeMessage/HeaderSection/TimeStamp, '[Y0001][M01][D01][H01]')" />
  </xsl:variable>

  <!-- функция -->
  <xsl:function name="func:FormatValString" as="xs:string">
    <xsl:param name="NameVal" as="xs:string"/>
    <xsl:param name="NameFormat" as="xs:string"/>

    <xsl:choose>
      <xsl:when test="$NameVal castable as xs:double">
        <xsl:sequence select="format-number(number($NameVal), $NameFormat)" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:sequence select="$NameVal" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:function>


  <xsl:template match="ExchangeMessage/DataSection">
    <xsl:variable name="v_46" select="sum(MeasureLines/MeasureLine/Properties/Property[SysName='flow' and (ExtKey!='57' or not(ExtKey))]/Value)"/>

    <xsl:if test="count(*/*/Properties/Property) &gt; 0">
      <xsl:for-each select="*/*/Properties/Property">
        <xsl:sort select="ExtKey"/>

        <xsl:if test="count(ExtKey) != 0">
          <xsl:if test="(SysName != 'dewPoint') or (Value  &lt; 999.0)">          
            <xsl:value-of select="ExtKey" />
            <xsl:text>:	  </xsl:text>

            <xsl:choose>
              <xsl:when test="ExtKey = '44'">
                <xsl:value-of select="func:FormatValString(string($v_46), '0.0' )"/>
              </xsl:when>
            
              <xsl:when test="SysName = 'dewPoint'">
                <xsl:if test="Value  &lt; 999.0">
                  <xsl:value-of select="func:FormatValString(string(Value), '0.0' )"/>
                </xsl:if>
              </xsl:when>    
            
              <xsl:when test="SysName = 'compressorUnitsInUse'">
                <xsl:value-of select="func:FormatValString(string(Value), '0' )"/>
              </xsl:when>               

              <xsl:when test="SysName = 'flow'">
                <xsl:value-of select="func:FormatValString(string(Value), '0.000' )"/>
              </xsl:when>   
              <xsl:otherwise>
                <xsl:value-of select="func:FormatValString(string(Value), '0.0' )"/>
              </xsl:otherwise>
            </xsl:choose>

            <xsl:text>	:</xsl:text>
            <xsl:value-of select="$timestamp" />
            <xsl:text>&#13;&#10;</xsl:text>
          </xsl:if>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>