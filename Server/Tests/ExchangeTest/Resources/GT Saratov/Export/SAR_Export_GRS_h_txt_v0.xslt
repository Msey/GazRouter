<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
  xmlns:func="http://MyFunction"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text"   encoding="windows-1251" />

  <xsl:decimal-format NaN="0" />

  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <!-- функция -->
  <xsl:function name="func:SetStringLong" as="xs:string">
    <xsl:param name="Long" as="xs:integer"/>
    <xsl:param name="NameVal" as="xs:string"/>
    <xsl:sequence select="string-join((for $i in 1 to ($Long - string-length($NameVal)) return '&#32;',$NameVal ) ,'')"/>
  </xsl:function>
  <!-- функция -->
  
  <xsl:template match="ExchangeMessage/HeaderSection">
    <xsl:variable name="timestamp" as ="xs:dateTime">
      <xsl:value-of select="TimeStamp" />
    </xsl:variable>
    <xsl:value-of select="format-dateTime($timestamp, '  [MNn] [D01],[Y0001]')" />
    <xsl:text>     Часовая реализация газа по ГРС П."ЮГТРАНСГАЗ"   </xsl:text>
    <xsl:value-of select="format-dateTime($timestamp, '[H1]:00')" />
    <xsl:text>&#13;&#10;--------------------------------------------------------------------------------&#13;&#10;      ЛПУ, ГРС     |  Факт   | Лимит | Откл. |         P          |     Т&#13;&#10;                   |     т.м.3/ч     |от лим.|   вх./вых./потр.   |  вх./вых.&#13;&#10;-------------------------------------------------------------------------------&#13;&#10;</xsl:text>
    
  </xsl:template>


  <xsl:template match="ExchangeMessage/DataSection">
    
    <xsl:if test="count(DistributingStations/DistributingStation) &gt; 0">
      <xsl:for-each select="DistributingStations/DistributingStation">
        <!-- Сортировка Ид ГРС-->
        <xsl:sort select="ExtKey"/>
        <!-- Ид ГРС во внешней системе , вида [3021]-->

        <!-- ЛПУ -->
        <xsl:variable name="value_LPU" select="upper-case(substring-before(substring-after(Item/Path, '/'), '/'))"/>
        <xsl:if test="substring(ExtKey,4,1) = 'L'">
          
          <xsl:variable name="len_LPU" select=" string-length($value_LPU) "/>
          <xsl:choose>
            <xsl:when test="$len_LPU &lt; 20">
              <xsl:value-of select="$value_LPU"/>
              <xsl:sequence select="string-join(for $i in 1 to (20-$len_LPU) return '&#32;','')"/>
              <!-- добавление нужного числа пробелов в конце записи имени ГРС-->
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="substring($value_LPU,1,20)"/>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:text>   0.000    0.000   0.000   0.0    0.0    0.0    0.0    0.0 &#13;&#10;</xsl:text>
        </xsl:if>

        <!-- Наименование ГРС-->
        <xsl:text>      </xsl:text>
        <xsl:variable name="len_name" select="string-length(Item/Name)"/>
        <xsl:choose>
          <xsl:when test="$len_name &lt; 13">
            <xsl:value-of select="Item/Name"/>
            <xsl:sequence select="string-join(for $i in 1 to (13-$len_name) return '&#32;','')"/>
            <!-- добавление нужного числа пробелов в конце записи имени ГРС-->
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="substring(Item/Name,1,13)"/>
            <xsl:text></xsl:text>
          </xsl:otherwise>
        </xsl:choose>

        <!-- Расход газа на ГРС-->

        <xsl:variable name="value_Q" select="format-number( Properties/Property[SysName='flow']/Value  ,'#0.000')"/>
        <xsl:value-of select="func:SetStringLong(9, $value_Q )"/>

        <!-- План Расход газа на ГРС-->
        <!-- Доработать -->        

        <xsl:variable name="value_PQ" select="format-number( Properties/Property[SysName='flow']/Value  ,'#0.000')"/>
        <xsl:value-of select="func:SetStringLong(9, $value_PQ )"/>

        <!-- Отклонение Расход газа на ГРС-->

        <xsl:variable name="value_dQ" select="format-number(number($value_PQ) - number($value_Q),'#0.000')"/>
        <xsl:value-of select="func:SetStringLong(8, $value_dQ )"/>        

        <!-- Давление на входе ГРС-->

        <xsl:variable name="value_pvh" select="format-number( Properties/Property[SysName='pressureInlet']/Value ,'#0.0' ) "/>
        <xsl:value-of select="func:SetStringLong(6, $value_pvh )"/>     

        <!-- Давление на выходе ГРС -->

        <xsl:variable name="value_pvih" select="format-number( ../../Outputs/Output[Item/ParentId=current()/Item/Id]/Properties/Property[SysName='pressureOutlet']/Value,'#0.0' ) "/>
        <xsl:value-of select="func:SetStringLong(7, $value_pvih)"/>     

        <!-- Давление на потребителе -->
        <!-- Доработать -->

        <xsl:variable name="value_pot" select="format-number( ../../Outputs/Output[Item/ParentId=current()/Item/Id]/Properties/Property[SysName='pressureOutlet']/Value,'#0.0' ) "/>
        <xsl:value-of select="func:SetStringLong(7, $value_pot )"/>

        <!-- Температура на входе ГРС-->

        <xsl:variable name="value_tvh" select="format-number( Properties/Property[SysName='temperatureInlet']/Value ,'#0.0') "/>
        <xsl:value-of select="func:SetStringLong(7, $value_tvh )"/>

        <!-- Температура на выходе ГРС-->

        <xsl:variable name="value_tvih" select="format-number( ../../Outputs/Output[Item/ParentId=current()/Item/Id]/Properties/Property[SysName='temperatureOutlet']/Value ,'#0.0' ) "/>
        <xsl:value-of select="func:SetStringLong(7, $value_tvih )"/>

        <xsl:text>&#13;&#10;</xsl:text>
        <!-- Добавить информацию по потребителям -->
        <!-- Доработать -->
      </xsl:for-each>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>