<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text" />
  <xsl:decimal-format NaN="" />
  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="ExchangeMessage/HeaderSection">
    <xsl:text>Газпром трансгаз Нижний Новгород&#10;</xsl:text>
  </xsl:template>

  <xsl:variable name="Date">
      <xsl:value-of select="format-dateTime(ExchangeMessage/HeaderSection/TimeStamp, '[D01][M01][Y0001]')" />
  </xsl:variable>
  <xsl:variable name="Seans">
      <xsl:value-of select="format-dateTime(ExchangeMessage/HeaderSection/TimeStamp, '[H01]')" />
  </xsl:variable>
  
  <xsl:template match="ExchangeMessage/DataSection">
    <xsl:if test="count(CompressorShops/CompressorShop) &gt; 0">
      <!-- КЦ-->
      <xsl:for-each select="CompressorShops/CompressorShop">
                    <xsl:value-of select="ExtKey" /> <xsl:text>;</xsl:text>
                    <xsl:value-of select="$Date" />  <xsl:text>;</xsl:text>
                    <xsl:value-of select="$Seans" /> <xsl:text>;</xsl:text>
                    <xsl:value-of select="Properties/Property[SysName='pressureInlet']/Value" /> <xsl:text>;</xsl:text>
                    <xsl:value-of select="Properties/Property[SysName='pressureOutlet']/Value" /> <xsl:text>;</xsl:text>
                    <xsl:value-of select="Properties/Property[SysName='temperatureInlet']/Value" /> <xsl:text>;</xsl:text>
                    <xsl:value-of select="Properties/Property[SysName='compressorUnitsInUse']/Value" /> <xsl:text>;</xsl:text>
                    <xsl:value-of select="Properties/Property[SysName='compressorUnitsInReserve']/Value" /> <xsl:text>;</xsl:text>
                    <xsl:value-of select="Properties/Property[SysName='temperatureOutlet']/Value" /> <xsl:text>;</xsl:text>
                    <xsl:value-of select="Properties/Property[SysName='temperatureCooling']/Value" /> <xsl:text>;</xsl:text>
                    <xsl:text>&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>

    <xsl:if test="count(CompressorStations/CompressorStation) &gt; 0">
      <!--  КС-->
      <xsl:for-each select="CompressorStations/CompressorStation">
          <xsl:value-of select="ExtKey" />
          <xsl:text>;</xsl:text>
          <xsl:value-of select="$Date" />
          <xsl:text>;</xsl:text>
          <xsl:value-of select="$Seans" />
          <xsl:text>;</xsl:text>
          <xsl:value-of select="Properties/Property[SysName='pressureAir']/Value" />
          <xsl:text>;</xsl:text>
          <xsl:value-of select="Properties/Property[SysName='temperatureAir']/Value" />
          <xsl:text>;</xsl:text>
          <xsl:text>&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>

    <xsl:if test="count(MeasuringStations/MeasuringStation) &gt; 0">
      <!--  ГИС -->
      <xsl:for-each select="MeasuringStations/MeasuringStation">
        <xsl:value-of select="ExtKey" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="$Date" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="$Seans" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="Properties/Property[SysName='flow']/Value" />
        <xsl:text>;</xsl:text>
        <xsl:text>&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>



