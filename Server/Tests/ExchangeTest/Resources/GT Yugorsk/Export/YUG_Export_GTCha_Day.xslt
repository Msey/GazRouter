<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text"  encoding="CP866" />

  <xsl:decimal-format NaN="" />

  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="ExchangeMessage/HeaderSection">
    <xsl:text>Тюментрансгаз"</xsl:text>
    <xsl:text>&#13;&#10;</xsl:text>
  </xsl:template>


  <xsl:template match="ExchangeMessage/DataSection">
    <xsl:variable name="timestamp">
      <xsl:value-of select="format-dateTime(xs:dateTime(../HeaderSection/TimeStamp) + xs:dayTimeDuration('P1D'), '[D01][M01][Y0001]')" />
    </xsl:variable>

    <xsl:for-each select="MeasureLines/MeasureLine/Properties/Property">
      <xsl:sort select="ExtKey"/>

      <xsl:if test="count(ExtKey) != 0">
        <xsl:value-of select="ExtKey" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="$timestamp" />
        <xsl:text>;24;</xsl:text>
        <xsl:choose>
          <xsl:when test="string(Value) castable as xs:double">
            <xsl:value-of select="format-number(Value *1000, '0')" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="Value" />
          </xsl:otherwise>
        </xsl:choose>
        <xsl:text>;&#13;&#10;</xsl:text>
      </xsl:if>
    </xsl:for-each>
    <xsl:text>44;</xsl:text>
    <xsl:value-of select="$timestamp" />
    <xsl:text>;24;;&#13;&#10;45;</xsl:text>
    <xsl:value-of select="$timestamp" />
    <xsl:text>;24;;&#13;&#10;END&#13;&#10;</xsl:text>
  </xsl:template>
</xsl:stylesheet>
