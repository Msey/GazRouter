<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text"  encoding="CP866" />

  <xsl:decimal-format NaN="0" />

  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="ExchangeMessage/DataSection">
    <xsl:if test="count(DistributingStations/DistributingStation) &gt; 0">
      <xsl:for-each select="DistributingStations/DistributingStation">
        <!-- Сортировка Ид ГРС-->
        <xsl:sort select="ExtKey"/>
        <!-- Ид ГРС во внешней системе , вида [3021]-->

        <xsl:text>[</xsl:text>
        <xsl:value-of select="substring(ExtKey,3)" />
        <xsl:text>]&#09;</xsl:text>
        <!-- символ табуляции-->

        <!-- Наименование ГРС-->

        <xsl:variable name="len_name" select="string-length(Item/Name)"/>
        <xsl:choose>
          <xsl:when test="$len_name &lt; 27">
            <xsl:value-of select="Item/Name"/>
            <xsl:sequence select="string-join(for $i in 1 to (27-$len_name) return '&#32;','')"/>
            <!-- добавление нужного числа пробелов в конце записи имени ГРС-->
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="substring(Item/Name,1,27)"/>
          </xsl:otherwise>
        </xsl:choose>
        <xsl:text>:</xsl:text>

        <!-- Давление на входе ГРС-->

        <xsl:variable name="value_pvh" select="format-number( Properties/Property[SysName='pressureInlet']/Value ,'#0.0' ) "/>
        <xsl:variable name="len_value_pvh" select=" string-length($value_pvh) "/>
        <xsl:sequence select="string-join(   (for $i in 1 to (5 - $len_value_pvh) return '&#32;',$value_pvh ) ,'')"/>

        <!-- Давление на выходе ГРС -->

        <xsl:variable name="value_pvih" select="format-number( ../../Outputs/Output[Item/ParentId=current()/Item/Id]/Properties/Property[SysName='pressureOutlet']/Value,'#0.0' ) "/>
        <xsl:variable name="len_value_pvih" select=" string-length($value_pvih) "/>
        <xsl:sequence select="string-join(   (for $k in 1 to (4 - $len_value_pvih) return '&#32;',$value_pvih ) ,'')"/>

        <!-- Температура на входе ГРС-->

        <xsl:variable name="value_tvh" select="format-number( Properties/Property[SysName='temperatureInlet']/Value ,'#0.0') "/>
        <xsl:variable name="len_value_tvh" select=" string-length($value_tvh) "/>
        <xsl:sequence select="string-join(   (for $i in 1 to (5 - $len_value_tvh) return '&#32;',$value_tvh ) ,'')"/>

        <!-- Температура на выходе ГРС-->

        <xsl:variable name="value_tvih" select="format-number( ../../Outputs/Output[Item/ParentId=current()/Item/Id]/Properties/Property[SysName='temperatureOutlet']/Value ,'#0.0' ) "/>
        <xsl:variable name="len_value_tvih" select=" string-length($value_tvih) "/>
        <xsl:sequence select="string-join(   (for $i in 1 to (5 - $len_value_tvih) return '&#32;',$value_tvih ) ,'')"/>

        <!-- Расход газа на ГРС-->

        <xsl:variable name="value_Q" select="format-number( Properties/Property[SysName='flow']/Value  ,'#0.000')"/>
        <xsl:variable name="len_value_Q" select=" string-length($value_Q) "/>
        <xsl:sequence select="string-join(   (for $i in 1 to (10 - $len_value_Q) return '&#32;',$value_Q ) ,'')"/>

        <xsl:text>&#13;&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>