<?xml version="1.0" encoding="utf-8"?>
<!-- Created with Liquid XML Studio Developer Edition (Trial) 9.1.9.3503 (http://www.liquid-technologies.com) -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="" xmlns:drop="http://microsoft.com/sat/drop.xsd">
    <xsl:template match="/drop:Drop">
        <Drop>
            <xsl:for-each select="*">
                <xsl:element name="Action">
                    <xsl:attribute name="name">
                        <xsl:value-of select="local-name(.)" />
                    </xsl:attribute>
                    <xsl:for-each select="*">
                        <xsl:call-template name="copy-element">
                            <xsl:with-param name="element" select="." />
                        </xsl:call-template>
                    </xsl:for-each>
                </xsl:element>
            </xsl:for-each>
        </Drop>
    </xsl:template>
    <!-- Copy Node: Primary Call -->
    <xsl:template name="copy-node">
        <xsl:param name="node" />
        <xsl:call-template name="copy-element">
            <xsl:with-param name="element" select="$node" />
        </xsl:call-template>
    </xsl:template>
    <xsl:template name="copy-element">
        <xsl:param name="element" />
        <xsl:if test="$element">
            <xsl:element name="{name($element)}">
                <!-- Copy Attributes -->
                <xsl:call-template name="copy-attribute">
                    <xsl:with-param name="attribute" select="$element/@*" />
                </xsl:call-template>
                <!-- Copy Text -->
                <xsl:value-of select="$element/text()" />
                <!-- Copy Child Nodes -->
                <xsl:for-each select="$element/*">
                    <xsl:call-template name="copy-node">
                        <xsl:with-param name="node" select="." />
                    </xsl:call-template>
                </xsl:for-each>
            </xsl:element>
        </xsl:if>
    </xsl:template>
    <!-- Copy Attribute -->
    <xsl:template name="copy-attribute">
        <xsl:param name="attribute" />
        <xsl:if test="$attribute">
            <xsl:attribute name="{name($attribute)}">
                <xsl:value-of select="$attribute" />
            </xsl:attribute>
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>
