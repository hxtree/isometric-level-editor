--[[
MQ:BtG boot
Copyright (c) illfoundedmind(tm) 2006
All Rights Reserved
coder(s): >__>
]]--
System.currentDirectory("ms0:/PSP/GAME/MQBtG/System/")

logo = Image.load("images/ifmlogo.png")
screen:blit(0,0,logo)
screen.flip()
System.sleep(1000)
logo = nil
screen:clear()
logo = Image.load("images/intro.png")
screen:blit(0,0,logo)
screen.flip()
local loop = true
while loop == true do
pad = Controls.read()
if pad:start() then loop = false end
end

logo = nil
screen:clear()

--[[

screen:clear()
for index, file in System.listDirectory() do
	screen:print(0,index*8,file.name,Color.new(255,255,255))
	screen:print(150,index*8,file.size,Color.new(255,255,255))
end

for a=0, 1000 do
	screen:print(0,200,"Copyright (c) illfoundedmind 2006",Color.new(255,255,255))
	screen:print(0,210,"All Rights Reserved",Color.new(255,255,255))
	screen:print(0,220,"LOADING...",Color.new(255,255,255))
end
	screen.flip()

System.sleep(1000)
]]--

System.currentDirectory("ms0:/PSP/GAME/MQBtG/System/menu")
dofile("libs/main.lib")



