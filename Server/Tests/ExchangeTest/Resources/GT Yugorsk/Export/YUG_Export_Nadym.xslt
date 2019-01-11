<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:func="http://MyFunction"
                xmlns:fn="http://www.w3.org/2005/xpath-functions">

  <xsl:output method="text" />

  <xsl:decimal-format NaN=""  decimal-separator="."/>
  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>
  <xsl:variable name="timestamp">
    <xsl:value-of select="format-dateTime(ExchangeMessage/HeaderSection/TimeStamp, '[Y0001][M01][D01][H01]')" />
  </xsl:variable>

  <xsl:variable name="tab_separator" >:</xsl:variable>
  <xsl:variable name="line_separator" >
    <xsl:text>&#13;&#10;</xsl:text>
  </xsl:variable>

  <!-- функция -->
  <xsl:function name="func:SetStringLong" as="xs:string">
    <xsl:param name="Long" as="xs:integer"/>
    <xsl:param name="NameVal"  />
    <xsl:sequence select="string-join((for $i in 1 to ($Long - string-length($NameVal)) return '&#32;',$NameVal ) ,'')"/>
  </xsl:function>

  <xsl:template match="ExchangeMessage/DataSection">
    <!-- compressorShops -->
    <xsl:if test="count(CompressorShops/CompressorShop) &gt; 0">
      <xsl:for-each select="CompressorShops/CompressorShop">
        <xsl:sort select="ExtKey"/>
        <!-- Давление на входе -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8, Properties/Property[SysName='pressureInlet']/ExtKey)" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(12,  format-number(Properties/Property[SysName='pressureInlet']/Value, '0.0'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>
        <!-- Температура на входе -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8,Properties/Property[SysName='temperatureInlet']/ExtKey)" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(12,  format-number(Properties/Property[SysName='temperatureInlet']/Value, '0.0'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>
        <!-- Давление на выходе -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8, Properties/Property[SysName='pressureOutlet']/ExtKey)" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(12,  format-number(Properties/Property[SysName='pressureOutlet']/Value, '0.0'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>
        <!-- Температура на выходе -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8, Properties/Property[SysName='temperatureOutlet']/ExtKey)" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(12,  format-number(Properties/Property[SysName='temperatureOutlet']/Value, '0.0'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>
        <!-- Степень сжатия -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8, Properties/Property[SysName='compressionRatio']/ExtKey)" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(12,  format-number(Properties/Property[SysName='compressionRatio']/Value, '0.00'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>
        <!-- Схема работы -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8, Properties/Property[SysName='compressorShopPattern']/ExtKey)" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(12,  format-number(Properties/Property[SysName='groupCount']/Value * Properties/Property[SysName='compressionStageCount']/Value, '0'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>
        <!-- Точка росы -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8, ../../MeasurePoints/MeasurePoint[Item/CompShopId=current()/Item/Id]/Properties/Property[SysName='dewPoint']/ExtKey)" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:variable name="t_ros">
          <xsl:value-of select="../../MeasurePoints/MeasurePoint[Item/CompShopId=current()/Item/Id]/Properties/Property[SysName='dewPoint']/Value" />
        </xsl:variable>
        <xsl:choose>
          <xsl:when test="replace($t_ros, ' ', '') >= '999'">
            <xsl:text>            </xsl:text>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="func:SetStringLong(12,  format-number($t_ros, '0.0'))" />
          </xsl:otherwise>
        </xsl:choose>
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>
        <!-- Температура воздуха -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8,  format-number(Properties/Property[SysName='pressureInlet']/ExtKey + 7, '0'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(12,  format-number(../../CompressorStations/CompressorStation[Item/Id=current()/Item/ParentId]/Properties/Property[SysName='temperatureAir']/Value, '0.0'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>

      </xsl:for-each>
    </xsl:if>
    <xsl:if test="count(ReducingStations/ReducingStation) &gt; 0">
      <xsl:for-each select="ReducingStations/ReducingStation">
        <xsl:sort select="ExtKey"/>
        <!-- Давление на входе -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8, Properties/Property[SysName='pressureInlet']/ExtKey)" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(12,  format-number(Properties/Property[SysName='pressureInlet']/Value, '0.0'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>
        <!-- Температура на входе -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8,Properties/Property[SysName='temperatureInlet']/ExtKey)" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(12,  format-number(Properties/Property[SysName='temperatureInlet']/Value, '0.0'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>
        <!-- Давление на выходе -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8, Properties/Property[SysName='pressureOutlet']/ExtKey)" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(12,  format-number(Properties/Property[SysName='pressureOutlet']/Value, '0.0'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>
        <!-- Температура на выходе -->
        <!-- внешний ключ -->
        <xsl:value-of select="func:SetStringLong(8, Properties/Property[SysName='temperatureOutlet']/ExtKey)" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(12,  format-number(Properties/Property[SysName='temperatureOutlet']/Value, '0.0'))" />
        <xsl:value-of select="$tab_separator"/>

        <xsl:value-of select="func:SetStringLong(11, $timestamp)"/>
        <xsl:value-of select="$tab_separator"/>
        <xsl:value-of select="$line_separator"/>
      </xsl:for-each>
    </xsl:if>


  </xsl:template>
</xsl:stylesheet>
