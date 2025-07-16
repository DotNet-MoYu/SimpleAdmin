export const ariaProps = {
	ariaHidden: Boolean,
	ariaRole: String,
	ariaLabel: String,
	ariaLabelledby: String,
	ariaDescribedby: String,
	ariaBusy: Boolean,
	// lStyle: String
}

export default {
	...ariaProps,
	lClass: String,
	name: {
		type: String,
		required: true,
	},
	color: String,
	size: [String, Number],
	prefix: String,
	// type: String,
	inherit: {
		type: Boolean,
		default: true
	},
	web: {
		type: Boolean,
		default: true
	},
	lStyle:[String, Object, Array],
}