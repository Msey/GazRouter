<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
  xmlns:func="http://MyFunction"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">

  <xsl:output method="text" />

  <xsl:decimal-format NaN=""  decimal-separator="."/>
  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>
  
  <!-- функция -->
  <xsl:function name="func:FormatValString" as="xs:string">
    <xsl:param name="NameVal" as="xs:string"/>
    <xsl:param name="NameFormat" as="xs:string"/>

    <xsl:choose>
      <xsl:when test="$NameVal castable as xs:double">
        <xsl:sequence select="format-number(number($NameVal), $NameFormat)" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:sequence select="$NameVal" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:function>  

  <xsl:variable name="timestamp">
    <xsl:value-of select="format-dateTime(ExchangeMessage/HeaderSection/TimeStamp, '[D01].[M01].[Y0001] [H01]:00:00')" />
  </xsl:variable>

  <xsl:template match="ExchangeMessage/DataSection">

    <xsl:if test="count(*/*/Properties/Property) &gt; 0">
      <xsl:for-each select="*/*/Properties/Property">
        <xsl:sort select="ExtKey"/>

        <xsl:if test="count(ExtKey) != 0">
          <xsl:value-of select="substring-after(ExtKey, ':')" />
          <xsl:text>,</xsl:text>

          <xsl:choose>
            <!-- Обработка кранов-->
            <xsl:when test="substring(ExtKey, 14, 1) = 'k'">
              <xsl:value-of select="format-dateTime(../Property[SysName='stateChangingTimestamp']/Value, '[D01].[M01].[Y0001] [H01]:[m01]:[s01]')" />
              <xsl:text>,</xsl:text>
              <xsl:text>,</xsl:text>              
              <xsl:value-of select="func:FormatValString(string(Value), '0' )"/>           
            </xsl:when>
            <xsl:otherwise>
            <!-- Обработка значений-->              
              <xsl:value-of select="$timestamp" />
              <xsl:text>,</xsl:text>
              
              <xsl:choose>
            <!-- Суммирование расходов-->            
                <xsl:when test="substring-after(ExtKey, ':') = 'GTU_S02C01Q1'">          
             
                  <xsl:variable name="Q_Val" select ="sum(/ExchangeMessage/DataSection/MeasureLines/MeasureLine[Item/ParentId=current()/../../Item/ParentId]/Properties/Property[SysName='flow']/Value)" />              
                  <xsl:value-of select="func:FormatValString(string($Q_Val), '0.0' )"/>
            
                </xsl:when>
            <!-- Суммирование агрегатов-->                       
                <xsl:when test="substring(ExtKey, 14) = 'AU'">          
             
                  <xsl:variable name="N1" select ="../Property[SysName='compressorUnitsInUse']/Value" />              
                  <xsl:variable name="N2" select ="../Property[SysName='compressorUnitsInReserve']/Value" />              
                  <xsl:variable name="N3" select ="Value" />              
                  
                  <xsl:variable name="s_Val" select ="$N1 + $N2 + $N3" />              
                  
                  <xsl:value-of select="func:FormatValString(string($s_Val), '0' )"/>
            
                </xsl:when>     
                <xsl:when test="substring(ExtKey, 14) = 'AR'">          
                  
                  <xsl:value-of select="func:FormatValString(string(Value), '0' )"/>
            
                </xsl:when>                   
                <xsl:otherwise>
            
                  <xsl:if test="(SysName != 'dewPoint') or (Value  &lt; 999.0)">
                    <xsl:value-of select="func:FormatValString(string(Value), '0.0' )"/>
                  </xsl:if>
      
                </xsl:otherwise>
              </xsl:choose>   
      
            </xsl:otherwise>
          </xsl:choose>
          
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
