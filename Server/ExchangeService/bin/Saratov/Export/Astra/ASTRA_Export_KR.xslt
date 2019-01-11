<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
	<xsl:output method="text" encoding="cp866" />
	
	<xsl:decimal-format NaN="" />
	
	<xsl:template match="text()">
		<xsl:apply-templates />
	</xsl:template>
	
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
				<xsl:variable name="e_key">
					<xsl:value-of select="substring-before(ExtKey, '::')" />
				</xsl:variable>
				<xsl:variable name="lpu_code">
					<xsl:value-of select="substring-after(ExtKey, '::')" />
				</xsl:variable>
				<xsl:value-of select="$e_key" />
				<xsl:text>, </xsl:text>
				
				<xsl:choose>
					<xsl:when test="Properties/Property[SysName='valveState']/Value=1">
						<xsl:text>00</xsl:text>
					</xsl:when>
					<xsl:when test="Properties/Property[SysName='valveState']/Value=2">
						<xsl:text>-1</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text></xsl:text>
					</xsl:otherwise>
				</xsl:choose>				
				<xsl:text>, </xsl:text>
				<xsl:value-of select="Properties/Property[SysName='stateBypass1']/Value" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="Properties/Property[SysName='stateBypass2']/Value" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="Properties/Property[SysName='stateBypass3']/Value" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-dateTime(Properties/Property[SysName='stateChangingTimestamp']/Value, '[D01].[M01].[Y0001]')" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-dateTime(Properties/Property[SysName='stateChangingTimestamp']/Value, '[H01].[m01]')" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="$lpu_code" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="Item/EntityType" />
				<xsl:text>:</xsl:text>
				<xsl:value-of select="Item/Id" />
				<xsl:text> </xsl:text>
				<xsl:value-of select="Item/ShortPath" />
				<xsl:text>&#13;&#10;</xsl:text>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
