<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:xs="http://www.w3.org/2001/XMLSchema" 	
  xmlns:fn="http://www.w3.org/2005/xpath-functions">

  <xsl:output method="text" />
 
  <xsl:decimal-format NaN=""  decimal-separator="."/>
  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template> 
  
  <xsl:template match="ExchangeMessage/DataSection">
      <!-- Замерные линии -->
      <xsl:for-each select="MeasureLines/MeasureLine">
         <xsl:variable name="measStationId">
			        <xsl:value-of select="Item/ParentId" />
		     </xsl:variable>
         <xsl:variable name="siteId">
			        <xsl:value-of select="/ExchangeMessage/DataSection/MeasuringStations/MeasuringStation[Item/Id=$measStationId]/Item/ParentId" />
		     </xsl:variable>         
         <xsl:variable name="siteExtKey">
			        <xsl:value-of select="/ExchangeMessage/DataSection/Sites/Site[Item/Id=$siteId]/ExtKey" />
		     </xsl:variable>    
        <xsl:for-each select="Properties/Property">           
            <xsl:choose>
              <xsl:when test="SysName = 'pressureInlet'">
                    <xsl:value-of select="$siteExtKey" />
                    <xsl:text>,GIS,</xsl:text>   
                    <xsl:value-of select="../../ExtKey" />
                    <xsl:text>,Pvh,</xsl:text>
                    <xsl:choose>
                      <xsl:when test="string(Value) castable as xs:double">
                        <xsl:value-of select="format-number(Value, '0.0')" />
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="Value" />
                      </xsl:otherwise>
                    </xsl:choose>
                    <xsl:text>, давление газа на входе   </xsl:text>
                    <xsl:value-of select="../../Item/ShortPath" />
                    <xsl:text>&#10;</xsl:text>
              </xsl:when>
              <xsl:when test="SysName = 'temperatureInlet'">
                <xsl:value-of select="$siteExtKey" />
                <xsl:text>,GIS,</xsl:text>   
                <xsl:value-of select="../../ExtKey" />
                <xsl:text>,Tvh,</xsl:text>
                <xsl:choose>
                <xsl:when test="string(Value) castable as xs:double">
                  <xsl:value-of select="format-number(Value, '0.0')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="Value" />
                </xsl:otherwise>
                </xsl:choose>
                <xsl:text>, температура газа на входе   </xsl:text>
                <xsl:value-of select="../../Item/ShortPath" />
                <xsl:text>&#10;</xsl:text>
              </xsl:when>
              <xsl:when test="SysName = 'flow'">
                <xsl:value-of select="$siteExtKey" />
                <xsl:text>,GIS,</xsl:text>   
                <xsl:value-of select="../../ExtKey" />
                <xsl:text>,Q,</xsl:text>
                <xsl:choose>
                  <xsl:when test="string(Value) castable as xs:double">
                    <xsl:value-of select="format-number(Value, '0.000')" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="Value" />
                  </xsl:otherwise>
                </xsl:choose>
                <xsl:text>, расход газа  </xsl:text>
                <xsl:value-of select="../../Item/ShortPath" />
                <xsl:text>&#10;</xsl:text>
              </xsl:when>
            </xsl:choose>          
        </xsl:for-each>
      </xsl:for-each>
      
      <!-- КЦ -->  
      <xsl:for-each select="CompressorShops/CompressorShop">
        <xsl:variable name="compStationId">
			        <xsl:value-of select="Item/ParentId" />
		     </xsl:variable>
         <xsl:variable name="siteId">
			        <xsl:value-of select="/ExchangeMessage/DataSection/CompressorStations/CompressorStation[Item/Id=$compStationId]/Item/ParentId" />
		     </xsl:variable>         
         <xsl:variable name="siteExtKey">
			        <xsl:value-of select="/ExchangeMessage/DataSection/Sites/Site[Item/Id=$siteId]/ExtKey" />
		     </xsl:variable>    
        <xsl:for-each select="Properties/Property">           
            <xsl:choose>
              <xsl:when test="SysName = 'pressureInlet'"> 
                <xsl:value-of select="$siteExtKey" />
                <xsl:text>,CEX,</xsl:text>
                <xsl:value-of select="../../ExtKey" />
                <xsl:text>,Pvh,</xsl:text>
                <xsl:choose>
                  <xsl:when test="string(Value) castable as xs:double">
                    <xsl:value-of select="format-number(Value, '0.0')" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="Value" />
                  </xsl:otherwise>
                </xsl:choose>
                <xsl:text>, давление газа на входе   </xsl:text>
                <xsl:value-of select="../../Item/ShortPath" />
                <xsl:text>&#10;</xsl:text>
              </xsl:when>
              <xsl:when test="SysName = 'temperatureInlet'">
                <xsl:value-of select="$siteExtKey" />
                <xsl:text>,CEX,</xsl:text>
                <xsl:value-of select="../../ExtKey" />
                <xsl:text>,Tvh,</xsl:text>
                <xsl:choose>
                  <xsl:when test="string(Value) castable as xs:double">
                    <xsl:value-of select="format-number(Value, '0.0')" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="Value" />
                  </xsl:otherwise>
                </xsl:choose>
                <xsl:text>, температура газа на входе   </xsl:text>
                <xsl:value-of select="../../Item/ShortPath" />
                <xsl:text>&#10;</xsl:text>
              </xsl:when>
              <xsl:when test="SysName = 'pressureOutlet'">
                <xsl:value-of select="$siteExtKey" />
                <xsl:text>,CEX,</xsl:text>
                <xsl:value-of select="../../ExtKey" />
                <xsl:text>,Pvih,</xsl:text>
                <xsl:choose>
                  <xsl:when test="string(Value) castable as xs:double">
                    <xsl:value-of select="format-number(Value, '0.0')" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="Value" />
                  </xsl:otherwise>
                </xsl:choose>
                <xsl:text>, давление газа на входе   </xsl:text>
                <xsl:value-of select="../../Item/ShortPath" />
                <xsl:text>&#10;</xsl:text>
              </xsl:when>
              <xsl:when test="SysName = 'temperatureOutlet'">
                <xsl:value-of select="$siteExtKey" />
                <xsl:text>,CEX,</xsl:text>
                <xsl:value-of select="../../ExtKey" />
                <xsl:text>,Tvih,</xsl:text>
                <xsl:choose>
                  <xsl:when test="string(Value) castable as xs:double">
                    <xsl:value-of select="format-number(Value, '0.0')" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="Value" />
                  </xsl:otherwise>
                </xsl:choose>
                <xsl:text>, давление газа на входе   </xsl:text>
                <xsl:value-of select="../../Item/ShortPath" />
                <xsl:text>&#10;</xsl:text>
              </xsl:when>
              <xsl:when test="SysName = 'compressorShopPattern'">
                <xsl:value-of select="$siteExtKey" />
                <xsl:text>,CEX,</xsl:text>
                <xsl:value-of select="../../ExtKey" />
                <xsl:text>,Scheme,</xsl:text>
                <xsl:value-of select="Value" />
                <xsl:text>, Схема работы </xsl:text> 
                <xsl:value-of select="../../Item/ShortPath" />
                <xsl:text>&#10;</xsl:text>
              </xsl:when>
            </xsl:choose>             
        </xsl:for-each>
      </xsl:for-each>

      <!-- ГРС -->
      <xsl:for-each select="DistributingStations/DistributingStation">
        <xsl:variable name="distrId">
			        <xsl:value-of select="Item/Id" />
		     </xsl:variable>    
         <xsl:variable name="siteId">
			        <xsl:value-of select="Item/ParentId" />
		     </xsl:variable>         
         <xsl:variable name="siteExtKey">
			        <xsl:value-of select="/ExchangeMessage/DataSection/Sites/Site[Item/Id=$siteId]/ExtKey" />
		     </xsl:variable>   
         <xsl:variable name="distrExtKey">
			        <xsl:value-of select="ExtKey" />
		     </xsl:variable>    
         
        <xsl:for-each select="Properties/Property">
          <xsl:choose>
            <xsl:when test="SysName = 'pressureInlet'">
              <xsl:value-of select="$siteExtKey" />
              <xsl:text>,GRS,</xsl:text>
              <xsl:value-of select="$distrExtKey" />
              <xsl:text>,Pvh,</xsl:text>
              <xsl:choose>
                <xsl:when test="string(Value) castable as xs:double">
                  <xsl:value-of select="format-number(Value, '0.0')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="Value" />
                </xsl:otherwise>
              </xsl:choose>
              <xsl:text>, давление газа на входе   </xsl:text>
              <xsl:value-of select="../../Item/ShortPath" />
              <xsl:text>&#10;</xsl:text>
            </xsl:when>
            <xsl:when test="SysName = 'temperatureInlet'">
              <xsl:value-of select="$siteExtKey" />
              <xsl:text>,GRS,</xsl:text>
              <xsl:value-of select="$distrExtKey" />
              <xsl:text>,Tvh,</xsl:text>
              <xsl:choose>
                <xsl:when test="string(Value) castable as xs:double">
                  <xsl:value-of select="format-number(Value, '0.0')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="Value" />
                </xsl:otherwise>
              </xsl:choose>
              <xsl:text>, температура газа на входе   </xsl:text>
              <xsl:value-of select="../../Item/ShortPath" />
              <xsl:text>&#10;</xsl:text>
            </xsl:when>
            <xsl:when test="SysName = 'flow'">
              <xsl:value-of select="$siteExtKey" />
              <xsl:text>,GRS,</xsl:text>
              <xsl:value-of select="$distrExtKey" />
              <xsl:text>,Q,</xsl:text>
              <xsl:choose>
                <xsl:when test="string(Value) castable as xs:double">
                  <xsl:value-of select="format-number(Value, '0.000')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="Value" />
                </xsl:otherwise>
              </xsl:choose>
              <xsl:text>, расход газа  </xsl:text>
              <xsl:value-of select="../../Item/ShortPath" />
              <xsl:text>&#10;</xsl:text>
            </xsl:when>
          </xsl:choose>       
        </xsl:for-each>
        <!--выходы ГРС-->                 
         <xsl:for-each select="/ExchangeMessage/DataSection/Outputs/Output[Item/ParentId=$distrId]">
            <xsl:for-each select="Properties/Property">
                  <xsl:choose>
                    <xsl:when test="SysName = 'pressureOutlet'">
                      <xsl:value-of select="$siteExtKey" />
                      <xsl:text>,GRS,</xsl:text>
                      <xsl:value-of select="$distrExtKey" />
                      <xsl:text>,Pvih,</xsl:text>
                      <xsl:choose>
                        <xsl:when test="string(Value) castable as xs:double">
                          <xsl:value-of select="format-number(Value, '0.0')" />
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="Value" />
                        </xsl:otherwise>
                      </xsl:choose>
                      <xsl:text>, давление газа на выходе ГРС   </xsl:text>
                      <xsl:value-of select="../../Item/ShortPath" />
                      <xsl:text>&#10;</xsl:text>
                    </xsl:when>
                    <xsl:when test="SysName = 'temperatureOutlet'">
                      <xsl:value-of select="$siteExtKey" />
                      <xsl:text>,GRS,</xsl:text>
                      <xsl:value-of select="$distrExtKey" />
                      <xsl:text>,Tvih,</xsl:text>
                      <xsl:choose>
                        <xsl:when test="string(Value) castable as xs:double">
                          <xsl:value-of select="format-number(Value, '0.0')" />
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="Value" />
                        </xsl:otherwise>
                      </xsl:choose>
                      <xsl:text>, температура газа на выходе   </xsl:text>
                      <xsl:value-of select="../../Item/ShortPath" />
                      <xsl:text>&#10;</xsl:text>
                    </xsl:when>
                  </xsl:choose>
                </xsl:for-each>          
         </xsl:for-each>       
      </xsl:for-each>

  </xsl:template>
</xsl:stylesheet>