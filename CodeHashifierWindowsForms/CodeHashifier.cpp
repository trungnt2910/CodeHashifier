#pragma GCC optimize ("O3")
#pragma GCC target("sse,sse2,sse3,ssse3,sse4,popcnt,abm,mmx,avx,tune=native")
#include <bits/stdc++.h>

#ifdef BUILDING_DLL
#include "dll.h"
#endif

using namespace std;

//Random Number and String generator
class Random
{
	private:
		static random_device impl;
	public:
		using result_type = random_device::result_type;
		static constexpr auto max = impl.max;
		static constexpr auto min = impl.min;
		//Returns 8-byte string:
		static string getString();
		//Returns 64-bit integer
		static int64_t getInt64();
		//Returns unsigned 64-bit integer
		static uint64_t getUInt64() {return (uint64_t)getInt64();}
		operator()() {return impl();}
};
random_device Random::impl;
string Random::getString()
{
	static char buffer[9] = {0, 0, 0, 0, 0, 0, 0, 0, 0};
	
	while (true)
	{
		buffer[0] = impl();
		if (isalpha(buffer[0])) break;
	}
	
	for (int i = 1; i < 8; ++i)
	{
		while (true)
		{
			buffer[i] = impl();
			if (isalnum(buffer[i])) break;
		}
	}
	
	return string(buffer);
}

int64_t Random::getInt64()
{
	static const std::size_t UIntsPerInt64 = ceil(1.0 * sizeof(int64_t) / sizeof(unsigned int));
	
	static unsigned int buffer[UIntsPerInt64];
	
	for (std::size_t i = 0; i < UIntsPerInt64; ++i)
	{
		buffer[i] = impl();
	}
	
	return (const int64_t &)(*((const int64_t *)(buffer)));
}

vector<bool> getStringContextVector(const string & str)
{
	bool isStringContext = false;
	bool isEscape = false;
	vector<bool> result(str.size());
	
	for (std::size_t i = 0; i < str.size(); ++i)
	{
		if ((!isEscape) && (str[i] == '\\')) isEscape = true;
		
		if ((!isEscape) && (str[i] == '\"')) isStringContext = !isStringContext;
		result[i] = isStringContext;
		
		if (isEscape) isEscape = false;
	}
	
	return result;
}

vector<bool> getCharLiteralContextVector(const string & str, const vector<bool> & stringContextVector)
{
	bool isCharLiteralContext = false;
	bool isEscape = false;
	vector<bool> result(str.size());
	
	for (std::size_t i = 0; i < str.size(); ++i)
	{
		if (stringContextVector[i])
		{
			isCharLiteralContext = false;
			result[i] = false;
			continue;
		}
		
		if ((!isEscape) && (str[i] == '\\')) isEscape = true;
		
		if ((!isEscape) && (str[i] == '\'')) isCharLiteralContext = !isCharLiteralContext;
		result[i] = isCharLiteralContext;
		
		if (isEscape) isEscape = false;
	}
	
	return result;
}

void flushCode(ostream & os, vector<string> & defines, vector<string> & code)
{
	static Random rand;
	
	//Refresh hashesPerLine each time this program flushes to increase randomness.
	int hashesPerLine = rand.getUInt64() % 12 + 4;
	
	if (!defines.empty())
	{
		shuffle(defines.begin(), defines.end(), rand);
		
		while (defines.size())
		{
			os << defines.back() << endl;
			defines.pop_back();
		}
	}
	
	if (!code.empty())
	{				
		for (int i = 0; i < code.size(); ++i)
		{
			int j = i;
			for (; j < min(i + hashesPerLine, (int)code.size()); ++j)
			{
				os << code[j] << " ";
			}
			os << endl;
			i = j - 1;
		}
		code.clear();
	}
}

#ifdef BUILDING_DLL
string HashMain(const string & source)
#else
int main(int argc, char ** argv)
#endif
{
	Random rand;
	
	//For the app, read from Console or File
#ifndef BUILDING_DLL
	string inputFileName = "";
	string outputFileName = "hacked.cpp";
	
	if (argc > 1) inputFileName = argv[1];
	if (argc > 2) outputFileName = argv[2];
	
	ifstream tempInputStream(inputFileName);
	
	istream & sourceFile = ((argc > 0) ? tempInputStream : cin);
	ofstream outputFile(outputFileName);
	
	//For the .dll, read data from arguments
#else
	stringstream sourceFile(source);
	stringstream outputFile;
#endif
	
	string line;
	bool isMultiLineComment = false;
	
	vector<map<string, string>> hashes;	
	hashes.push_back(map<string, string>());

	vector<string> defines;
	vector<string> code;
	
	while (!sourceFile.eof())
	{
		getline(sourceFile, line); 
		
		vector<bool> stringContextIdentifier = getStringContextVector(line);
		vector<bool> characterContextIdentifier = getCharLiteralContextVector(line, stringContextIdentifier);
		
		//Remove evil multiline comments
		if (isMultiLineComment)
		{
			size_t multiLineCommentEnd = line.find("*/");
			if ((multiLineCommentEnd != string::npos))
			{
				isMultiLineComment = false;
				line = string(line.begin() + multiLineCommentEnd + 2, line.end());
			}
			else continue;
		}
		
		if (!isMultiLineComment)
		{
			size_t multiLineCommentBegin = line.find("/*");
			if ((multiLineCommentBegin != string::npos) && (!stringContextIdentifier[multiLineCommentBegin]))
			{
				isMultiLineComment = true;
				size_t multiLineCommentEnd = line.find("*/");
				if (multiLineCommentEnd != string::npos)
				{
					isMultiLineComment = false;
					line.erase(line.begin() + multiLineCommentBegin, line.begin() + multiLineCommentEnd + 2);
				}
				else
				{
					line = string(line.begin(), line.begin() + multiLineCommentBegin);
				}
			}
		}
		
		//Remove comments:
		size_t inlineCommentBegin = line.find("//");
		if ((inlineCommentBegin != string::npos) && (!stringContextIdentifier[inlineCommentBegin]))
		{
			line = string(line.begin(), line.begin() + inlineCommentBegin);
		}
		
		//Ignore empty lines after comments stripped:
		if (line.empty()) continue;
		
		int contentStart = 0;
		while ((contentStart < line.size()) && (isspace(line[contentStart]))) ++contentStart;
		line = string(line.begin() + contentStart, line.end());
		
		//Ignore empty lines after leading whitespaces removed:
		if (line.empty()) continue;
		
		while (isspace(line.back())) line.pop_back();
		
		//Ignore empty lines (again):
		if (line.empty()) continue;
			
		//Ignore macros
		if (line[0] == '#') 
		{
			//Flush our #defines first else they may affect our macros
			//And also flush our code else our other macros may affect it
			
			flushCode(outputFile, defines, code);
			
			outputFile << line << endl;
			
			//#if preprocessor macros produces child scopes which may break previous hashified builds
			//#if, #ifdef, #ifndef
			if (line.substr(0, 3) == "#if")
			{
				hashes.push_back(map<string, string>());
			}
			//#else, #elif
			else if (line.substr(0, 3) == "#el")
			{
				hashes.pop_back();
				hashes.push_back(map<string, string>());
			}
			else if (line.substr(0, 6) == "#endif")
			{
				hashes.pop_back();
			}
			
			continue;
		}
		
		while ((line.size() > 0) && (isspace(line.back()))) line.pop_back();
				
		stringstream lineStream(line);
		
		string currentWord;
				
		while (!lineStream.eof())
		{
			lineStream >> currentWord;
			
			//Eat the whole line if it contains a string or a character literal
			if ((currentWord.find('\"') != string::npos) || (currentWord.find('\'') != string::npos))
			{
				size_t startpos = line.find(currentWord);
				
				while (lineStream) lineStream >> currentWord;
				
				currentWord = string(line.begin() + startpos, line.end());
			}
			
			auto it = hashes.back().cend();
			
			for (const auto & hash : hashes)
			{
				it = hash.find(currentWord);
				if (it != hash.end())
				{
					break;
				}
			}
			
			if (it != hashes.back().cend())
			{
				code.push_back(it->second);
			}
			else
			{
				hashes.back()[currentWord] = rand.getString();
				defines.push_back("#define " + hashes.back()[currentWord] + " " + currentWord);
				code.push_back(hashes.back()[currentWord]);
			}
		}		
	}

	//Flush defines the last time:
	flushCode(outputFile, defines, code);
	
	hashes.pop_back();
	
#ifdef BUILDING_DLL
	string resultString = outputFile.str();
	while ((resultString.size()) && (isspace(resultString.back()))) resultString.pop_back();
	return resultString;
#else
	return 0;
#endif
}

#ifdef BUILDING_DLL
const char * __stdcall Hash(const char * source)
{
	static char * result = nullptr;
	
	string resultString = HashMain(source);
	
	delete [] result;
	
	//One for the null character
	result = new char[resultString.size() + 1];
	for (std::size_t i = 0; i < resultString.size(); ++i)
	{
		result[i] = resultString[i];
	}
	result[resultString.size()] = '\0';
	std::cerr << resultString << endl;
	return result;
}

const char * __stdcall GetVersion()
{
	static const string version = "0.0.1 Beta";
	static const string buildTimeStamp = __DATE__ + string(" ") + __TIME__;
	static const string VersionString = "LibCodeHashifier " + version + "\nBuilt on " + buildTimeStamp;
	return VersionString.c_str();
}
#endif
