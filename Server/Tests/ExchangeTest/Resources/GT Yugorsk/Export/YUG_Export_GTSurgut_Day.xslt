<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
  xmlns:func="http://MyFunction"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text"   encoding="CP866"  />

  <xsl:decimal-format NaN=" " />

  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <!-- функция -->
  <xsl:function name="func:SetStringLongL" as="xs:string">
    <xsl:param name="Long" as="xs:integer"/>
    <xsl:param name="NameVal" as="xs:string"/>
    <xsl:sequence select="string-join((for $i in 1 to ($Long - string-length($NameVal)) return '&#32;',$NameVal ) ,'')"/>
  </xsl:function>

  <xsl:function name="func:SetStringLongR" as="xs:string">
    <xsl:param name="Long" as="xs:integer"/>
    <xsl:param name="NameVal" as="xs:string"/>
    <xsl:sequence select="string-join(($NameVal, for $i in 1 to ($Long - string-length($NameVal)) return '&#32;' ) ,'')"/>
  </xsl:function>

  <xsl:template match="ExchangeMessage/HeaderSection">
    <xsl:variable name="timestamp" as ="xs:dateTime">
      <xsl:value-of select="TimeStamp" />
    </xsl:variable>
    <xsl:text>Тюментрансгаз,  </xsl:text>

    <xsl:variable name="timestamp2">
      <xsl:value-of select="format-dateTime(xs:dateTime(TimeStamp) + xs:dayTimeDuration('P1D'), '[D01]/[M01]/[Y0001]')" />
    </xsl:variable>

    <xsl:value-of select="$timestamp2" />
    <xsl:text>, режим на  00</xsl:text>
    <xsl:text>&#13;&#10;</xsl:text>
  </xsl:template>

  <xsl:template match="ExchangeMessage/DataSection">
    <xsl:if test="count(CompressorShops/CompressorShop) &gt; 0">
      <!-- КЦ-->
      <xsl:for-each select="CompressorShops/CompressorShop">
        <xsl:variable name="value_EK" select="substring-before(ExtKey, '::')"/>
        <xsl:value-of select="func:SetStringLongL(2, $value_EK )"/>
        <xsl:text>;</xsl:text>

        <xsl:variable name="value_PI" select="format-number( Properties/Property[SysName='pressureInlet']/Value  ,'#0.0')"/>
        <xsl:value-of select="func:SetStringLongL(5, $value_PI )"/>
        <xsl:text>;</xsl:text>

        <xsl:variable name="value_PO" select="format-number( Properties/Property[SysName='pressureOutlet']/Value  ,'#0.0')"/>
        <xsl:value-of select="func:SetStringLongL(5, $value_PO )"/>
        <xsl:text>;</xsl:text>

        <xsl:variable name="value_TI" select="format-number( Properties/Property[SysName='temperatureInlet']/Value  ,'#0.0')"/>
        <xsl:value-of select="func:SetStringLongL(5, $value_TI )"/>
        <xsl:text>;</xsl:text>

        <xsl:variable name="value_TC" select="format-number( Properties/Property[SysName='temperatureCooling']/Value  ,'#0.0')"/>
        <xsl:value-of select="func:SetStringLongL(5, $value_TC )"/>
        <xsl:text>;</xsl:text>

        <xsl:variable name="value_GC" select="format-number( Properties/Property[SysName='groupCount']/Value  ,'#0')"/>
        <xsl:value-of select="func:SetStringLongL(2, $value_GC )"/>
        <xsl:text>;</xsl:text>

        <xsl:variable name="value_CSC" select="format-number( Properties/Property[SysName='compressionStageCount']/Value  ,'#0')"/>
        <xsl:value-of select="func:SetStringLongL(2, $value_CSC )"/>
        <xsl:text>;</xsl:text>

        <xsl:variable name="value_SP" select="Item/StationName"/>
        <xsl:value-of select="func:SetStringLongR(20, $value_SP )"/>
        <xsl:text>;</xsl:text>

        <!--<xsl:variable name="value_PL" select="Item/PipelineName"/>-->
        <xsl:value-of select="func:SetStringLongR(20, substring-after(ExtKey, '::') )"/>
        <xsl:text>;</xsl:text>

        <xsl:text>&#13;&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>


</xsl:stylesheet>
