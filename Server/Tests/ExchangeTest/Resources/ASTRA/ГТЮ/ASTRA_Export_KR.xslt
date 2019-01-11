<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
  xmlns:func="http://MyFunction"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text"  encoding="CP866" />

  <xsl:decimal-format NaN="" />

  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <!-- функция -->
  <xsl:function name="func:SetStringLong" as="xs:string">
    <xsl:param name="Long" as="xs:integer"/>
    <xsl:param name="NameVal" as="xs:string"/>
    <xsl:sequence select="string-join( ($NameVal, for $i in 1 to ($Long - string-length($NameVal)) return '&#32;' ) ,'')"/>
  </xsl:function>


  <xsl:function name="func:GetValveState" as="xs:string">
    <xsl:param name="Valve" as="xs:string"/>
    <xsl:choose>
      <xsl:when test="$Valve='1'">
        <xsl:text>00</xsl:text>
      </xsl:when>
      <xsl:when test="$Valve='2'">
        <xsl:text>-1</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text></xsl:text>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:function>

  <xsl:template match="ExchangeMessage/HeaderSection">
    <xsl:variable name="timestamp">
      <xsl:value-of select="format-dateTime(TimeStamp, '[D01].[M01].[Y0001]')" />
    </xsl:variable>
    <xsl:value-of select="$timestamp" />
    <xsl:text>&#13;&#10;</xsl:text>
  </xsl:template>

  <xsl:template match="ExchangeMessage/DataSection">
    <xsl:if test="count(Valves/Valve) &gt; 0">
      <xsl:for-each select="Valves/Valve">
        <xsl:sort select="ExtKey"/>

        <xsl:variable name="e_key">
          <xsl:value-of select="substring-before(ExtKey, '::')" />
        </xsl:variable>
        <xsl:variable name="lpu_code">
          <xsl:value-of select="substring-after(ExtKey, '::')" />
        </xsl:variable>
        <xsl:value-of select="func:SetStringLong(10, $e_key )" />
        <xsl:text>,</xsl:text>

        <xsl:value-of select="func:GetValveState(string(Properties/Property[SysName='valveState']/Value))" />
        <xsl:text>,</xsl:text>
        <xsl:value-of select="func:GetValveState(string(Properties/Property[SysName='stateBypass1']/Value))" />
        <xsl:text>,</xsl:text>
        <xsl:value-of select="func:GetValveState(string(Properties/Property[SysName='stateBypass2']/Value))" />
        <xsl:text>,</xsl:text>
        <xsl:value-of select="func:GetValveState(string(Properties/Property[SysName='stateBypass3']/Value))" />
        <xsl:text>,</xsl:text>
        <xsl:value-of select="format-dateTime(Properties/Property[SysName='stateChangingTimestamp']/Value, '[D01].[M01].[Y0001]')" />
        <xsl:text>,</xsl:text>
        <xsl:value-of select="format-dateTime(Properties/Property[SysName='stateChangingTimestamp']/Value, '[H01].[m01]')" />
        <xsl:text>,</xsl:text>
        <xsl:value-of select="$lpu_code" />
        <xsl:text>,</xsl:text>
        <xsl:value-of select="Item/EntityType" />
        <xsl:text>:</xsl:text>
        <xsl:value-of select="Item/Id" />
        <xsl:text> </xsl:text>
        <!--xsl:value-of select="Item/ShortPath" /-->
        <xsl:text>&#13;&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
