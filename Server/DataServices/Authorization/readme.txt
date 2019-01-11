Для включения аутентификации Windows в asp.net в Local IIS Web server. Необходимо  отредактировать applicationhost.config:
1) в <sectionGroup name="authentication"> должно быть выставлено overrideModeDefault в allow
				<section name="anonymousAuthentication" overrideModeDefault="Allow" />
                    <section name="basicAuthentication" overrideModeDefault="Allow" />
                    <section name="windowsAuthentication" overrideModeDefault="Allow" />

2)в      <authentication>


                <anonymousAuthentication enabled="false" userName="" />

                <basicAuthentication enabled="true" />

                           <windowsAuthentication enabled="true">
                    <providers>
                        <add value="Negotiate" />
                        <add value="NTLM" />
                    </providers>
                </windowsAuthentication>

