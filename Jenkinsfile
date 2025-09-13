pipeline {
    agent any
    environment {
        PROJECT_NAME = "PetroBM"
        HOST_SERVER = "Server104Dev"
        HOSTED_SITE = "KHO_TKV_DEV"
        // this path should be set correctly to avoid removal of wrong folders        
        HOSTED_SITE_FILE_PATH = "\"D:\\SOURCE_DEV_BY_CICD\\${HOSTED_SITE}\""
        PUBLISH_PROFILE = "FolderProfile"
        PUBLISH_PROFILE_BUILD_RESULT = "\\PetroBM.Web\\publish"
        DEFAULT_REPORT_RECEIVER = "dluongta@"
        BUILD_TOOL = "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2022\\BuildTools\\MSBuild\\Current\\Bin\\msbuild.exe\""
        PACKAGE_TOOL = "\"D:\\Tools\\nuget.exe\""
    }

    stages {
        
        stage("build") {
            steps {                
                bat "${BUILD_TOOL} ${PROJECT_NAME}.sln /t:clean /verbosity:quiet"
                bat "${PACKAGE_TOOL} restore"
                bat "IF EXIST .${PUBLISH_PROFILE_BUILD_RESULT} RMDIR /S /Q .${PUBLISH_PROFILE_BUILD_RESULT}"
                bat "${BUILD_TOOL} ${PROJECT_NAME}.sln /t:restore /verbosity:quiet"
                bat "${BUILD_TOOL} ${PROJECT_NAME}.sln /p:DeployOnBuild=true /p:PublishProfile=${PUBLISH_PROFILE} /verbosity:quiet"
            }
        }

        stage("deploy") {
            steps {
                script {
                    executeStepsOfDeploymentStage()
                }
            }
        }
    }

    // post {
        // always {
        //     emailext body: '''${SCRIPT, template="groovy-html.template"}''',
        //         mimeType: 'text/html',
        //         recipientProviders: [developers(), requestor(), culprits()], 
        //         subject: "[Jenkins] BUILD REPORT OF RESULT ${currentBuild.result}", 
        //         to: "${DEFAULT_REPORT_RECEIVER}"
        // }
    // }
}

def executeStepsOfDeploymentStage() {
    def webServerCommand = "C:\\Windows\\System32\\inetsrv\\appcmd.exe"
    
    useSSHPublisher('', "${webServerCommand} stop site ${HOSTED_SITE}", '')
    useSSHPublisher('', "${webServerCommand} stop apppool ${HOSTED_SITE}", '')
    useSSHPublisher('', "IF NOT EXIST ${HOSTED_SITE_FILE_PATH} MKDIR ${HOSTED_SITE_FILE_PATH}", '')
    useSSHPublisher('', "CD /D ${HOSTED_SITE_FILE_PATH} && DEL /F /Q *.*", '')
    useSSHPublisher('', "CD /D ${HOSTED_SITE_FILE_PATH} && FOR /D %I IN (*.*) DO (RMDIR /S /Q \"%I\")", '')
    useSSHPublisher("${PUBLISH_PROFILE_BUILD_RESULT}\\**\\*", '', PUBLISH_PROFILE_BUILD_RESULT)
    useSSHPublisher('', "${webServerCommand} start apppool ${HOSTED_SITE}", '')
    useSSHPublisher('', "${webServerCommand} start site ${HOSTED_SITE}", '')
}

def useSSHPublisher(def uploadFiles, def command, def removedPrefix) {
    sshPublisher(publishers: [sshPublisherDesc(configName: HOST_SERVER, 
        transfers: [sshTransfer(sourceFiles: uploadFiles, cleanRemote: true, excludes: '', 
        execCommand: command, execTimeout: 120000, flatten: false, 
        makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', 
        remoteDirectory: HOSTED_SITE, remoteDirectorySDF: false, removePrefix: removedPrefix)],
        usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false)])
}