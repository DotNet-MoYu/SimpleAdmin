<template>
	<view style="padding: 100rpx;">
		<text>lime-svg uniapp 1</text>
		<!-- <l-svg style="width: 50rpx; height: 50rpx;" src="/static/logo.svg" @click="onClick"></l-svg> -->
		<!-- <l-svg style="width: 150rpx;height: 150rpx;"  src="/static/svg/a.svg" @click="onClick"></l-svg> -->
		<l-svg style="width: 150rpx;height: 150rpx;" src="/static/svg/a.svg" color="red" @click="onClick"></l-svg>
		<l-svg style="width: 150rpx;height: 150rpx;" color="blue" src="https://api.iconify.design/material-symbols/backpack-sharp.svg"></l-svg>
		<l-svg style="width: 150rpx;height: 150rpx;" src="https://www.xmplus.cn/uploads/images/20221228/b9e9d45054ab5795992a1e92584a278b.svg"></l-svg>
		<l-svg style="width: 150rpx;height: 150rpx;font-size: none;" color="red" src='<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M6 15h1.5V9H5v1.5h1zm2.5 0H13V9H8.5zm1.5-1.5v-3h1.5v3zm4 1.5h1.5v-2.25L17.25 15H19l-2.25-3L19 9h-1.75l-1.75 2.25V9H14zM3 21V3h18v18z"/></svg>'></l-svg>
		<l-svg style="width: 150rpx;height: 150rpx;" color="blue" src="data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxZW0iIGhlaWdodD0iMWVtIiB2aWV3Qm94PSIwIDAgMjQgMjQiPjxwYXRoIGZpbGw9ImN1cnJlbnRDb2xvciIgZD0iTTYgMTVoMS41VjlINXYxLjVoMXptMi41IDBIMTNWOUg4LjV6bTEuNS0xLjV2LTNoMS41djN6bTQgMS41aDEuNXYtMi4yNUwxNy4yNSAxNUgxOWwtMi4yNS0zTDE5IDloLTEuNzVsLTEuNzUgMi4yNVY5SDE0ek0zIDIxVjNoMTh2MTh6Ii8+PC9zdmc+"></l-svg>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				
			}
		},
		mounted() {
			let t = Date.now()
			uni.request({
				url: 'https://api.iconify.design/material-symbols/backpack-sharp.svg?v=1',
				dataType: 'text',
				success: (res) => {
					console.log('end:::', Date.now() - t)
				},
				fail: () => {
					
				}
			})	
		},
		methods: {
			onClick() {
				console.log('click')
			}
		}
	}
</script>

<style>

</style>