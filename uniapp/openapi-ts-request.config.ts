import type { GenerateServiceProps } from 'openapi-ts-request'

export default [
  {
    schemaPath: 'http://localhost:5566/swagger/Default/swagger.json',
    serversPath: './src/service/sys',
    requestLibPath: `import request from '@/utils/request';\n import { CustomRequestOptions } from '@/http/interceptor';`,
    requestOptionsType: 'CustomRequestOptions',
    isGenReactQuery: true,
    reactQueryMode: 'vue',
    isGenJavaScript: false,
  },
  {
    schemaPath: 'http://localhost:5566/swagger/Mobile/swagger.json',
    serversPath: './src/service/mobile',
    requestLibPath: `import request from '@/utils/request';\n import { CustomRequestOptions } from '@/http/interceptor';`,
    requestOptionsType: 'CustomRequestOptions',
    isGenReactQuery: true,
    reactQueryMode: 'vue',
    isGenJavaScript: false,
  },
] as GenerateServiceProps[]
