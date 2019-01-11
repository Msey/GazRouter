<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" 	xmlns:fn="http://www.w3.org/2005/xpath-functions">

  <xsl:output method="text" />

  <xsl:decimal-format NaN=""  decimal-separator="."/>
  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>
  <xsl:variable name="timestamp">
    <xsl:value-of select="format-dateTime(xs:dateTime(ExchangeMessage/HeaderSection/TimeStamp)  + xs:dayTimeDuration('P1D'), '[Y0001][M01][D01]12')" />
  </xsl:variable>
  <xsl:template match="ExchangeMessage/HeaderSection" >
    <xsl:variable name="genTimestamp">
      <xsl:value-of select="format-dateTime(GeneratedTime, '[Y0001][M01][D01][H01][m01][s01]')" />
    </xsl:variable>
    <xsl:text>  13:	</xsl:text>
    <xsl:value-of select="$genTimestamp" />
    <xsl:text>&#13;&#10;</xsl:text>
  </xsl:template>

  <xsl:template match="ExchangeMessage/DataSection/*/*/Properties/Property">
    <xsl:if test="count(ExtKey) != 0">
      <xsl:value-of select="ExtKey" />
      <xsl:text>:	  </xsl:text>
      <xsl:choose>
        <xsl:when test="string(Value) castable as xs:double">
          <xsl:value-of select="format-number(Value, '0.0')" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="Value" />
        </xsl:otherwise>
      </xsl:choose>
      <xsl:text>	:</xsl:text>
      <xsl:value-of select="$timestamp" />
      <xsl:text>&#13;&#10;</xsl:text>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
