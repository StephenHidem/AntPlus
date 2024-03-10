<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://wixtoolset.org/schemas/v4/wxs"
    xmlns="http://wixtoolset.org/schemas/v4/wxs"
    version="1.0"
    exclude-result-prefixes="xsl wix">

	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />
	<xsl:strip-space elements="*" />

	<!-- Remove .exe files from harvested files -->
	<xsl:key
        name="ExeToRemove"
        match="wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 3 ) = '.exe' ]"
        use="@Id"
    />

	<!-- We can also remove .pdb files too. -->
	<xsl:key
        name="PdbToRemove"
        match="wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 3 ) = '.pdb' ]"
        use="@Id"
    />

	<!-- By default, copy all elements and nodes into the output... -->
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" />
		</xsl:copy>
	</xsl:template>

	<!-- ...but if the element has the "ExeToRemove" key then don't render anything (i.e. removing it from the output) -->
	<xsl:template match="*[ self::wix:Component or self::wix:ComponentRef ][ key( 'ExeToRemove', @Id ) ]" />
	<xsl:template match="*[ self::wix:Component or self::wix:ComponentRef ][ key( 'PdbToRemove', @Id ) ]" />
</xsl:stylesheet>