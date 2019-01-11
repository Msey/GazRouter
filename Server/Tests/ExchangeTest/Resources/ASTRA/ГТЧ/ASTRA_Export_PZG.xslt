<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
	<xsl:output method="text" />
	
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
		<!-- PZG -->
		<xsl:if test="count(DistributingStations/DistributingStation) &gt; 0">
			<xsl:for-each select="DistributingStations/DistributingStation">
				<!--<xsl:value-of select="format-number(ExtKey,'0000')" />-->
				<xsl:choose>
					<xsl:when test="string(ExtKey) castable as xs:double">
						<xsl:value-of select="format-number(ExtKey,'0000')" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="ExtKey" />
					</xsl:otherwise>
				</xsl:choose>
			  
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-number(Properties/Property[SysName='flow']/Value, '0.0############################################')" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-number(Properties/Property[SysName='pressureInlet']/Value, '0.0')" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-number(Properties/Property[SysName='temperatureInlet']/Value, '0.0')" />
				<xsl:text>, ,, </xsl:text>
				<xsl:value-of select="Item/EntityType" />
				<xsl:text>:</xsl:text>
				<xsl:value-of select="Formatted_ID" />
				<xsl:text> </xsl:text>
				<xsl:value-of select="Item/ShortPath" />
				<xsl:text>&#13;&#10;</xsl:text>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="count(MeasureLines/MeasureLine) &gt; 0">
			<xsl:for-each select="MeasureLines/MeasureLine">
				<!--<xsl:value-of select="format-number(ExtKey,'0000')" />-->
				<xsl:choose>
					<xsl:when test="string(ExtKey) castable as xs:double">
						<xsl:value-of select="format-number(ExtKey,'0000')" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="ExtKey" />
					</xsl:otherwise>
				</xsl:choose>
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-number(Properties/Property[SysName='flow']/Value, '0.0############################################')" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-number(Properties/Property[SysName='pressureInlet']/Value, '0.0')" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-number(Properties/Property[SysName='temperatureInlet']/Value, '0.0')" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-number(../../MeasurePoints/MeasurePoint[Item/MeasLineId=current()/Item/Id]/Properties/Property[SysName='densityAbsolute']/Value, '0.0')" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-number(../../MeasurePoints/MeasurePoint[Item/MeasLineId=current()/Item/Id]/Properties/Property[SysName='combustionHeatLow']/Value, '0.0')" />
				<xsl:text>, ,, </xsl:text>
				<xsl:value-of select="Item/EntityType" />
				<xsl:text>:</xsl:text>
				<xsl:value-of select="Formatted_ID" />
				<xsl:text> </xsl:text>
				<xsl:value-of select="Item/ShortPath" />
				<xsl:text>&#13;&#10;</xsl:text>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>