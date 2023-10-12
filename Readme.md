# Setup

`winget install Microsoft.DotNet.SDK.6`

`winget install Microsoft.DotNet.DesktopRuntime.6`

`winget install Microsoft.DotNet.AspNetCore.6`

`winget install Microsoft.DotNet.Runtime.6`

`dotnet new -i Amazon.Lambda.Templates`

`dotnet tool install -g Amazon.Lambda.Tools`

`dotnet tool install -g Amazon.Lambda.TestTool-6.0` Optional if you're goint to attemp local debugging
    See: https://github.com/aws/aws-lambda-dotnet/tree/master/Tools/LambdaTestTool#configure-for-jetbrains-rider for more details

`dotnet new serverless.Annotations --name performance.api.redis.a`

To get a lambda function access to the internet then additional steps need to be followed. High level, you need to create an Internet Gateway and for address 0.0.0.0 traffic (Route Table) and associate that route with a public subnet in the VPC.  Additionally you'll need to create a NAT Gateway and a new Route Table, in the new route table you'll add a route for address 0.0.0.0 traffic using the NAT Gateway, this route will be associated with private subnets in the VPC.

# Deploy

Once the `aws-lamda-tools-defaults.json file` is setup with the preferred params, you can deploy using `dotnet lambda deploy-serverless`, the mentioned json file is used a the default config file.

Function specific deployment details are in the `serverless.template` file.  This is where you set security group and subnets.

More specific detail regarding the template and aws annotations can be found in the project folder's readme